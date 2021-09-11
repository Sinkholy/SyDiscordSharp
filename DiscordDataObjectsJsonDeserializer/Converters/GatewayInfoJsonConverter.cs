using System;

using API;

using DiscordDataObjectsDeserializer;

using Newtonsoft.Json.Linq;

namespace DiscordDataObjectsJsonDeserializer.Converters
{
	public class GatewayInfoJsonConverter : BasicTypeJsonConverter<GatewayInfo>
    {
        const string ErrorDuringSessionLimitsDeserialization = "An error occurred while parsing a nested object.";
        const string ErrorDuringShardsParsing = "An error occurred when converting string to number.";

        const string UrlPropKey = "url";
        const string ShardsPropKey = "shards";
        const string SessionStartLimitPropKey = "session_start_limit";
        readonly DiscordDataObjectsJsonConverter converter;

        public GatewayInfoJsonConverter(DiscordDataObjectsJsonConverter converter)
        {
            this.converter = converter;
            var sessionStartLimitsConverter = new SessionStartLimitsJsonConverter();
            converter.AddNewConvertableType(sessionStartLimitsConverter);
        }

        public override bool CanSerialize => false;
        public override bool CanDeserialize => true;

        public override ConversionResult<GatewayInfo> Deserialize(string serialized)
        {
            if(!TryToParse(serialized, out JObject jObject, out var exception))
			{
                return ConversionResult<GatewayInfo>.Failed(ConversionErrorEnum.InputDataError, exception.Message);
			}

			var sessionLimitsSerialized = jObject[SessionStartLimitPropKey]?.ToString();
			var sessionLimitsConversion = converter.Deserialize<GatewayInfo.SessionStartLimit>(sessionLimitsSerialized);
			GatewayInfo.SessionStartLimit sessionLimits;
			if (sessionLimitsConversion.IsSuccessfull)
			{
				sessionLimits = sessionLimitsConversion.Result;
			}
			else
			{
				return ConversionResult<GatewayInfo>.Failed(ConversionErrorEnum.UnknownType, 
                                                            ErrorDuringSessionLimitsDeserialization, 
                                                            sessionLimitsConversion.Error);
			}
			var url = jObject[UrlPropKey]?.ToObject<Uri>();// TODO: потцениальное место для ошибок, не находишь?
                                                           // Мб стоит обернуть в try catch?
            var shardsSerialized = jObject[ShardsPropKey]?.ToString();
            bool shardIsParsed = int.TryParse(shardsSerialized, out int shards);
            if (!shardIsParsed)
            {
                return ConversionResult<GatewayInfo>.Failed(ConversionErrorEnum.InputDataError, ErrorDuringShardsParsing);
            }

            var deserialized = new GatewayInfo(url, shards, sessionLimits);
            return ConversionResult<GatewayInfo>.Successfull(deserialized);
        }
        public override ConversionResult<string> Serialize(GatewayInfo serializable)
        {
            throw new NotImplementedException();
        }

        public class SessionStartLimitsJsonConverter : BasicTypeJsonConverter<GatewayInfo.SessionStartLimit>
        {
            const string ErrorDuringDataConversion = "An error occurred when converting the input data.";

            const string TotalPropKey = "total";
            const string RemainingPropKey = "remaining";
            const string ResetAfterPropKey = "reset_after";
            const string MaxConcurrencyPropKey = "max_concurrency";

            public override bool CanSerialize => false;
            public override bool CanDeserialize => true;

            public override ConversionResult<GatewayInfo.SessionStartLimit> Deserialize(string serialized)
            {
                if (!TryToParse(serialized, out JObject jObject, out var exception))
                {
                    return ConversionResult<GatewayInfo.SessionStartLimit>.Failed(ConversionErrorEnum.InputDataError, exception.Message);
                }

                var totalSerialized = jObject[TotalPropKey]?.ToString();
                bool totalIsParsed = int.TryParse(totalSerialized, out int total);
                if (!totalIsParsed)
                {
                    return ConversionResult<GatewayInfo.SessionStartLimit>.Failed(ConversionErrorEnum.InputDataError, ErrorDuringDataConversion);
                }

                var remainingSerialized = jObject[RemainingPropKey]?.ToString();
                bool remainingIsParsed = int.TryParse(remainingSerialized, out int remainig);
                if (!remainingIsParsed)
                {
                    return ConversionResult<GatewayInfo.SessionStartLimit>.Failed(ConversionErrorEnum.InputDataError, ErrorDuringDataConversion);
                }

                var resetAfterSerialized = jObject[ResetAfterPropKey]?.ToString();
                bool resetAfterIsParsed = int.TryParse(resetAfterSerialized, out int resetAfterInt);
                if (!resetAfterIsParsed)
                {
                    return ConversionResult<GatewayInfo.SessionStartLimit>.Failed(ConversionErrorEnum.InputDataError, ErrorDuringDataConversion);
                }
                var resetAfter = TimeSpan.FromMilliseconds(resetAfterInt); // TODO: потцениальное место для ошибок, не находишь?
                                                                           // Мб стоит обернуть в try catch?

                var maxConcurrencySerialized = jObject[MaxConcurrencyPropKey]?.ToString();
                bool maxConcurrencyIsParsed = int.TryParse(maxConcurrencySerialized, out int maxConcurrency);
                if (!maxConcurrencyIsParsed)
                {
                    return ConversionResult<GatewayInfo.SessionStartLimit>.Failed(ConversionErrorEnum.InputDataError, ErrorDuringDataConversion);
                }

                var deserialized = new GatewayInfo.SessionStartLimit(total, remainig, resetAfter, maxConcurrency);
                return ConversionResult<GatewayInfo.SessionStartLimit>.Successfull(deserialized);
            }

            public override ConversionResult<string> Serialize(GatewayInfo.SessionStartLimit serializable)
            {
                throw new NotImplementedException();
            }
        }
    }
}
