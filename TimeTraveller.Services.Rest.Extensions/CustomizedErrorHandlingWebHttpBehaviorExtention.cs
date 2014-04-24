using System;
using System.ServiceModel.Configuration;

namespace TimeTraveller.Services.Rest.Extensions
{
    public class CustomizedErrorHandlingWebHttpBehaviorExtention : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(CustomizedErrorHandlingWebHttpBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new CustomizedErrorHandlingWebHttpBehavior();
        }
    }
}
