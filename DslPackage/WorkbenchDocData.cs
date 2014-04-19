using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luminis.Its.Workbench.DslPackage
{
    partial class WorkbenchDocData
    {
        public void InitFromITS()
        {
            try
            {
                CreateModelingDocStore(CreateStore());
                var t = Store.TransactionManager.BeginTransaction();
                SetRootElement(new ModelRoot(Store));
                t.Commit();
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
