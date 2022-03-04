using DotNetCore.CAP.Filter;

namespace WebAppCAPDemo.Filter
{
    public class MyCapFilter : SubscribeFilter
    {
        public override void OnSubscribeExecuting(ExecutingContext context)
        {
            // 订阅方法执行前
            System.Console.WriteLine("订阅方法执行前");
        }

        public override void OnSubscribeExecuted(ExecutedContext context)
        {
            // 订阅方法执行后
            System.Console.WriteLine("订阅方法执行后");
        }

        public override void OnSubscribeException(ExceptionContext context)
        {
            System.Console.WriteLine("订阅方法执行异常");
            return;
            // 订阅方法执行异常
        }
    }
}
