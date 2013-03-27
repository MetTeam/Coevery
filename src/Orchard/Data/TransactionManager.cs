﻿using System.Data;
using System.Web.Mvc;
using Orchard.Mvc.Filters;

namespace Orchard.Data {
    public interface ITransactionManager : IDependency {
        void Demand();
        void RequireNew(IsolationLevel level = IsolationLevel.ReadCommitted);
        void Cancel();
    }

    public class TransactionFilter : FilterProvider, IExceptionFilter {
        private readonly ITransactionManager _transactionManager;

        public TransactionFilter(ITransactionManager transactionManager) {
            _transactionManager = transactionManager;
        }

        public void OnException(ExceptionContext filterContext) {
            _transactionManager.Cancel();
        }
    }
}
