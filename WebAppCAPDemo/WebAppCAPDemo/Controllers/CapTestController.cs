using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System;
using System.Data.SqlClient;
using WebAppCAPDemo.Filter;
using static WebAppCAPDemo.CAPDbcontext;

namespace WebAppCAPDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CapTestController : ControllerBase
    {
        private readonly ICapPublisher _capPublisher;
        private readonly CAPDbcontext _cAPDbcontext;

        public CapTestController(ICapPublisher capPublisher, CAPDbcontext cAPDbcontext)
        {
            _capPublisher = capPublisher;
            _cAPDbcontext = cAPDbcontext;
        }

        #region CAP 简单的控制器测试

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        [HttpGet("testPubCap1")]
        public IActionResult testCap1()
        {
            _capPublisher.Publish("testSubCap1", DateTime.Now);

            return Ok("Ok");
        }

        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="time"></param>
        [HttpGet("testSubCap1")]
        [CapSubscribe("testSubCap1")]
        private void testSubCap1(DateTime time)
        {
            var aa = 0;
            var bb = 1;
            ////报错1
            //throw new Exception("故意报错");
            ////报错2
            var cc = bb / aa;//故意报错
            Console.WriteLine("当前时间:" + time);
        }

        /// <summary>
        /// 判断消费者的回调
        /// </summary>
        /// <param name="time"></param>
        [HttpGet("testSubCap2")]
        [CapSubscribe("testSubCap2")]
        private void testSubCap2(string msg)
        {
            if (msg == "ok")
            {
                Console.WriteLine("消费者消费成功了");
            }
            else
            {
                Console.WriteLine("消费者消费失败了");
            }
        }

        #endregion

        /// <summary>
        /// 数据库 事务方面的使用 EF
        /// </summary>
        /// <returns></returns>
        [HttpGet("testDBTransAction1")]
        [TypeFilter(typeof(CapTransactionFilter))]
        public IActionResult testDBTransAction1([FromServices] CAPDbcontext dbContext)
        {
            _cAPDbcontext.Add<test>(new test
            {
                Name = "张三1",
                Age = 181
            });

            var result = _cAPDbcontext.SaveChanges();

            //这个始终最后调用
            _capPublisher.Publish("testSubCap1", DateTime.Now, "testSubCap2");

            //这里异常了 为什么还能继续执行下去呢? 并且数据记录已经插入进去了 !!!!!!
            //保存
            return Ok("Ok");
        }


        /// <summary>
        /// 数据库 事务方面的使用 ADO.NET
        /// </summary>
        /// <returns></returns>
        [HttpGet("testDBTransAction")]
        public IActionResult testDBTransAction()
        {
            using (var connection = new MySqlConnection("Server=42.192.3.5;port=3306;Database=testcap;User ID=root;Password=123456;charset=utf8;Allow User Variables=true"))
            {
                using (var transaction = connection.BeginTransaction(_capPublisher))
                {
                    //new MyCapFilter(transaction);

                    var addQuery = "insert into test(name,age) values('zhangsan1',181);";
                    //1.以下是数据库版本差异
                    //sqlserver 
                    //SqlCommand cmd = new SqlCommand(addQuery, connection, transaction);

                    //mysql ||PostgreSql
                    MySqlCommand cmd = new MySqlCommand(addQuery, connection, (MySqlTransaction)transaction.DbTransaction);
                    cmd.ExecuteNonQuery();
                    //这个始终最后调用
                    _capPublisher.Publish("testSubCap1", DateTime.Now, "testSubCap2");
                    //这里异常了 为什么还能继续执行下去呢? 并且数据记录已经插入进去了 !!!!!!
                    //保存
                }
            }

            return Ok("Ok");
        }
    }
}
