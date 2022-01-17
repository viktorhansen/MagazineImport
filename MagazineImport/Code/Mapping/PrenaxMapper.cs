using MagazineImport.Code.Importers;
using System;
using System.Data;
using System.Globalization;

namespace MagazineImport.Code.Mapping
{
    public class PrenaxMapper : MagazineMapperDefaults
    {
        private readonly string FileName;
        private readonly DataRow DataSource;
        private object Field(string column)
        {
            return DataSource != null && DataSource.Table.Columns.Contains(column) ? DataSource[column] : string.Empty;
        }

        public PrenaxMapper(DataRow dr, string strFileName)
        {
            FileName = strFileName;
            DataSource = dr;
        }

        private readonly string _importJobSuffix;
        private int _id = 0;
        public override int Id { get { return _id; } set { _id = value; } }
        public override string ImportJobId { get { return "Prenax" + _importJobSuffix; } }
        public override string ExternalId { get { return Convert.ToString(Field("ProductId")); } }
        public override string Issn { get { return Convert.ToString(Field("ISSN")); } }
        public override string ProductName { get { return Convert.ToString(Field("Title")); } }
        public override string Description { get { return Convert.ToString(Field("Description")); } }
        public override string FirstIssue { get { return "4-14"; } }
        public override int FreqPerYear { get { return Convert.ToInt32(Convert.ToString(Field("Frequency")) != "NULL" && !string.IsNullOrEmpty(Convert.ToString(Field("Frequency"))) ? Field("Frequency") : 0); } }

        public override string ExternalOfferId { get { return Convert.ToString(Field("DeliveryOptionId")); } }
        public override string CountryName { get { return Convert.ToString(Field("PublisherCountry")); } }
        public override string OfferIdPrepaid { get { return Convert.ToString(Field("DeliveryOptionId")); } }
        public override string CampaignIdPrepaid { get { return Convert.ToString(Field("DeliveryOption")); } }
        public override int SubscriptionLength
        {
            get
            {
                int length;
                if (int.TryParse(Convert.ToString(Field("OfferLength")), out length))
                    return length;

                return 0;
            }
        }
        public override decimal InPrice { get { return Convert.ToDecimal(Field("Price").ToString().Replace(",", "."), CultureInfo.InvariantCulture); } }
        public override decimal Price { get { return Convert.ToDecimal(Field("Price").ToString().Replace(",", "."), CultureInfo.InvariantCulture); } }
        public override string ExtraInfo { get { return string.Empty; } }
        public override int CurrencyId
        {
            get
            {
                switch (Convert.ToString(Field("Valuta")))
                {
                    case "SEK":
                        return 1;
                    case "NOK":
                        return 2;
                    case "EUR":
                        return 3;
                    default:
                        return 1;
                }
            }
        }
        public override int CountryId
        {
            get
            {
                if (FileName.StartsWith(PrenaxImporter.strFilePrefix + "se_"))
                    return 1;
                if (FileName.StartsWith(PrenaxImporter.strFilePrefix + "no_"))
                    return 2;
                if (FileName.StartsWith(PrenaxImporter.strFilePrefix + "fi_"))
                    return 13;
                if (FileName.StartsWith(PrenaxImporter.strFilePrefix + "al_"))
                    return 35;

                return 1;
            }
        }
    }
}
