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
            _logger = logger;
        }

        public void UpdateDataFromContext(Credentials creds, Source source)
        {
            // need logger, db credentials and context source object

            PostProcBuilder ppb = new PostProcBuilder(source, _logger);
            ppb.EstablishContextForeignTables(creds);
            ppb.EstablishTempNamesTable();

            // if pubmed (or includes pubmedd, as with expected test data), do these updates first
            if (source.id == 100135 || source.id == 999999)
            {
                ppb.ObtainPublisherInformation();
                ppb.ApplyPublisherData();
                _logger.Information("Updating Publisher Info\n");
            }

            // Update and standardise organisation ids and names

            if (source.has_study_tables || source.source_type == "test")
            {
                ppb.UpdateStudyIdentifierOrgs();
                _logger.Information("Study identifier orgs updated");

                if (source.has_study_contributors)
                {
                    ppb.UpdateStudyContributorOrgs();
                    _logger.Information("Study contributor orgs updated");
                }

                ppb.StoreUnMatchedNamesForStudies();
                _logger.Information("Unmatched org names for studies stored");
            }

            if (source.source_type == "object" || source.source_type == "test")
            {
                // works at present in the context of PubMed - may need changing 

                ppb.UpdateObjectIdentifierOrgs();
                _logger.Information("Object identifier orgs updated");

                ppb.UpdateObjectContributorOrgs();
                _logger.Information("Object contributor orgs updated");

                ppb.StoreUnMatchedNamesForObjects();
                _logger.Information("Unmatched org names for objects stored");
            }

            ppb.UpdateDataObjectOrgs();
            _logger.Information("Data object managing orgs updated");

            ppb.StoreUnMatchedNamesForDataObjects();
            _logger.Information("Unmatched org names in data objects stored");


            // Update and standardise topic ids and names
            ppb.UpdateTopics(source.source_type);
            _logger.Information("Topic data updated");

            // Tidy up...
            ppb.DropTempNamesTable();
            ppb.DropContextForeignTables();

        }


    }
}
