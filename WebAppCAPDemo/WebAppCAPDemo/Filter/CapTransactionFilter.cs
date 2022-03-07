using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppCAPDemo.Filter
{
    public class CapTransactionFilter : TypeFilterAttribute
    {
        public CapTransactionFilter() : base(typeof(TransactionActionFilter))
        {

        }

        public class TransactionActionFilter : IActionFilter
        {
            private IDbContextTransaction _transaction;

            public void OnActionExecuting(ActionExecutingContext context)
            {
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<CAPDbcontext>();
                var capPublisher = context.HttpContext.RequestServices.GetService<ICapPublisher>();
                _transaction = dbContext.Database.BeginTransaction(capPublisher);
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                if (context.Exception == null)
                {
                    _transaction.Commit();
                }
                else
                {
                    _transaction.Rollback();
                }
                _transaction?.Dispose();
            }
        }
    }
}
