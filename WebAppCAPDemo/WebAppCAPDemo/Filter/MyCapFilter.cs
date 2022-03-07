using DotNetCore.CAP;
using DotNetCore.CAP.Filter;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System;

namespace WebAppCAPDemo.Filter
{
    public class MyCapFilter : SubscribeFilter
    {
        private readonly CAPDbcontext _dbContext;
        private IDbContextTransaction _transaction;

        public MyCapFilter(CAPDbcontext cAPDbcontext)
        {
            _dbContext = cAPDbcontext;
        }
        public override void OnSubscribeExecuting(ExecutingContext context)
        {
            // 订阅方法执行前
            System.Console.WriteLine("订阅方法执行前");
            _transaction = _dbContext.Database.BeginTransaction();
        }

        public override void OnSubscribeExecuted(ExecutedContext context)
        {
            // 订阅方法执行后
            System.Console.WriteLine("订阅方法执行后");
            _transaction.Commit();
        }

        public override void OnSubscribeException(ExceptionContext context)
        {
            // 订阅方法执行异常
            System.Console.WriteLine("订阅方法执行异常");
            _transaction.Rollback();
        }
    }
}
