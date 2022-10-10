using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.ValueObject
{
    public class Result<T>
    {
        public Result()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.";
            Title = "One or more validation errors occurred.";
            Status = 400;
            TraceId = Guid.NewGuid().ToString();
        }

        public Result(T content)
        {
            Content = content;
        }

        [JsonIgnore]
        public T Content { get; set; }

        [JsonProperty("type")]
        public string Type { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("status")]
        public int Status { get; private set; }

        [JsonProperty("traceId")]
        public string TraceId { get; private set; }

        [JsonProperty("errors")]
        public IDictionary<string, IList<string>> Errors { get; set; }

        [JsonIgnore]
        public bool HasError { get { return Errors?.Count > 0; } }
               
        public void AddError(string prefix, string error)
        {
            if (Errors == null)
            {
                Errors = new Dictionary<string, IList<string>>();
            }

            if (!Errors.ContainsKey(prefix))
            {
                Errors.Add(prefix, new List<string>());
            }

            Errors[prefix].Add(error);
        }
    }
}
