using System;
using System.IO;
using System.Linq;
using TimeTraveller.Services.CaseFiles;
using TimeTraveller.Services.CaseFileSpecifications;
using TimeTraveller.Services.Rules;

namespace TimeTraveller.Services.Rest.Impl.Commands.CaseFiles
{
    public abstract class AbstractGetCaseFileCommand : AbstractCaseFileCommand, ICommand
    {
    	#region private
    	private IRuleService _ruleService;
    	#endregion
    	
    	#region properties
    	protected IRuleService RuleService
    	{
    		get	{ return _ruleService; }
    	}
    	#endregion
        
    	#region Constructors
        public AbstractGetCaseFileCommand(ICaseFileSpecificationService caseFileSpecificationService, ICaseFileService caseFileService, IRuleService ruleService)
            : base(caseFileSpecificationService, caseFileService)
        {
        	_ruleService = ruleService;
        }
        #endregion

        #region Abstract Members
        public abstract CaseFile GetCaseFile(CaseFileSpecification caseFileSpecification, string caseFileId, CommandContext context);
        public abstract Rule GetRule(CommandContext context);
        #endregion
        
        #region protected
        protected CaseFile Execute(Rule rule, CaseFile caseFile, CaseFileSpecification caseFileSpecification)
        {
            CaseFile resultingCaseFile = this.RuleService.Execute(rule, caseFile);
            return resultingCaseFile;
        }
        #endregion
        
        #region ICommand Members
        public override Stream Execute(CommandContext context, IFormatter formatter)
        {
            string specificationId = string.Format(_caseFileSpecificationTemplate, context.Arguments[0], context.Arguments[1]);
            CaseFileSpecification caseFileSpecification = _caseFileSpecificationService.Get(specificationId, context.BaseUri);
            if (caseFileSpecification == null)
            {
                throw new ArgumentOutOfRangeException("specificationname", string.Format("Unknown specification {0}/{1} in uri {1}", context.Arguments[0], context.Arguments[1], context.RequestUri));
            }

            string caseFileId = GetCaseFileId(caseFileSpecification, context);
            CaseFile caseFile = GetCaseFile(caseFileSpecification, caseFileId, context);
			
            // We have a CaseFile, does it need transformation before being formatted?
            if (context.QueryParameters.AllKeys.Any<string>(k => k == "transform"))
            {            	
            	// Yep it does. Now create a new Command
            	string rulename = context.QueryParameters["transform"].ToString();
            	CommandContext ruleContext = new CommandContext(context.WebOperationContext, typeof(Rule), context.Arguments[0], context.Arguments[1], rulename);

            	Rule rule = GetRule(ruleContext);
            	caseFile = Execute(rule, caseFile, caseFileSpecification);
            }

            Stream result = formatter.Format(context, caseFile);

            return result;
        }
        #endregion
    }
}
