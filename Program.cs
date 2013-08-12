using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using log4net;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using Microsoft.ContentManagement.Publishing;

namespace Escc.Cms.PageDeleter
{
    class Program
    {
        // REMEMBER: if copying logging code, set assembly attribute for Log4Net in AssemblyInfo.cs
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            // Get channels to clear out from app.config
            NameValueCollection config = ConfigurationManager.GetSection("Escc.Cms.PageDeleter/Channels") as NameValueCollection;
            if (config == null || config.Count == 0)
            {
                Console.WriteLine(Properties.Resources.ErrorNoChannels);
                return;
            }

            CmsApplicationContext cms = null;
            try
            {
                // Log app start
                log.Info(Properties.Resources.LogStart);

                // Get CMS context
                cms = new CmsApplicationContext();
                cms.AuthenticateAsCurrentUser(PublishingMode.Update);
                bool somethingDeleted = false;

                // Config key should be a channel GUID
                foreach (string key in config.AllKeys)
                {
                    // Optionally, channel GUID could be followed by a comma and a template GUID
                    string templateGuid = String.Empty;
                    string channelGuid = key;
                    int commaPos = channelGuid.IndexOf(",", StringComparison.Ordinal);
                    if (commaPos > -1)
                    {
                        templateGuid = channelGuid.Substring(commaPos + 1);
                        channelGuid = channelGuid.Substring(0, commaPos);
                    }

                    // Get channel
                    Channel ch = cms.Searches.GetByGuid(channelGuid) as Channel;
                    if (ch != null)
                    {
                        // Log channel
                        log.Info(String.Format(CultureInfo.CurrentCulture, Properties.Resources.LogChannel, ch.UrlModePublished));

                        // Config value should be a date property and the number of days old.
                        // Turn into a negative so that we can "add" it to the current date.
                        try
                        {
                            string property = config[key];
                            int days = 0;
                            commaPos = property.IndexOf(",", StringComparison.Ordinal);
                            if (commaPos > -1)
                            {
                                days = Int32.Parse("-" + property.Substring(commaPos + 1), CultureInfo.CurrentCulture);
                                property = property.Substring(0, commaPos).ToUpperInvariant();
                            }

                            DateTime cutOff = DateTime.Now.AddDays(days);

                            // Check each posting for whether it was created before that date; if so, delete it
                            foreach (Posting p in ch.Postings)
                            {
                                var oldEnoughToDelete = false;
                                if (property == "CREATEDDATE") oldEnoughToDelete = (p.CreatedDate < cutOff);
                                else if (property == "LASTMODIFIEDDATE") oldEnoughToDelete = (p.LastModifiedDate < cutOff);

                                if (oldEnoughToDelete && (templateGuid.Length == 0 || templateGuid == p.Template.Guid))
                                {
                                    string updateMessage = String.Format(CultureInfo.CurrentCulture, Properties.Resources.LogDeleted, p.UrlModePublished);

                                    p.Delete();
                                    somethingDeleted = true;

                                    // Log change
                                    log.Info(updateMessage);
                                }
                            }
                        }
                        catch (FormatException ex)
                        {
                            // Update console
                            Console.WriteLine(Properties.Resources.ErrorNumericFormat);

                            // Publish error
                            ExceptionManager.Publish(ex);

                            // Log error
                            log.Error(ex.Message);

                            return;
                        }
                        catch (OverflowException ex)
                        {
                            // Update console
                            Console.WriteLine(Properties.Resources.ErrorNumericFormat);

                            // Publish error
                            ExceptionManager.Publish(ex);

                            // Log error
                            log.Error(ex.Message);

                            return;
                        }

                    }
                }

                // Nothing actually deleted until we call CommitAll
                if (somethingDeleted)
                {
                    cms.CommitAll();
                }
            }
            catch (Exception ex)
            {
                // Update console
                Console.WriteLine(ex.Message);

                // Publish error
                ExceptionManager.Publish(ex);

                // Log error
                log.Error(ex.Message);

            }
            finally
            {
                if (cms != null) cms.Dispose();

                // Log finish
                log.Info(Properties.Resources.LogStop);
            }
        }
    }
}
