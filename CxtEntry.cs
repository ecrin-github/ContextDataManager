using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ContextDataManager
{
    public class ContextMain
    {
        // Used to set up the parameters required for the procedures
        // and then run each of them
        ILogger _logger;

        public ContextMain(ILogger logger)
        {
            // need logger, db credentials and context source object
            _logger = logger;
        }

        public void UpdateDataFromContext(Credentials creds, Source source)
        {
            PostProcBuilder ppb = new PostProcBuilder(source, _logger);
            ppb.EstablishContextForeignTables(creds);

            // Update and standardise organisation ids and names
            _logger.Information("Updating Orgs and Topics\n");
            if (source.has_study_tables)
            {
                ppb.UpdateStudyIdentifierOrgs();
                _logger.Information("Study identifier orgs updated");
                ppb.UpdateStudyContributorOrgs();
                _logger.Information("Study contributor orgs updated");
            }
            ppb.UpdateDataObjectOrgs();
            _logger.Information("Data object managing orgs updated");
            ppb.StoreUnMatchedNames();
            _logger.Information("Unmatched org names stored");

            // Update and standardise topic ids and names
            string source_type = source.has_study_tables ? "study" : "object";
            ppb.UpdateTopics(source_type);
            _logger.Information("Topic data updated");

            // if necessary update publisher details (PubvMed data only)
            // or do this seperately as timing may vary...

            ppb.DropContextForeignTables();

        }


    }
}
