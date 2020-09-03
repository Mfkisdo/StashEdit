using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StashEdit.Class
{
    public class Posters
    {

        [JsonProperty("large")]
        public string large { get; set; }

        [JsonProperty("medium")]
        public string medium { get; set; }

        [JsonProperty("small")]
        public string small { get; set; }
    }

    public class Background
    {

        [JsonProperty("full")]
        public string full { get; set; }

        [JsonProperty("large")]
        public string large { get; set; }

        [JsonProperty("medium")]
        public string medium { get; set; }

        [JsonProperty("small")]
        public string small { get; set; }
    }

    public class Extra
    {

        [JsonProperty("gender")]
        public string gender { get; set; }

        [JsonProperty("birthday")]
        public string birthday { get; set; }

        [JsonProperty("iafd")]
        public object iafd { get; set; }

        [JsonProperty("astrology")]
        public object astrology { get; set; }

        [JsonProperty("birthplace")]
        public string birthplace { get; set; }

        [JsonProperty("ethnicity")]
        public object ethnicity { get; set; }

        [JsonProperty("nationality")]
        public object nationality { get; set; }

        [JsonProperty("haircolor")]
        public object haircolor { get; set; }

        [JsonProperty("height")]
        public string height { get; set; }

        [JsonProperty("weight")]
        public string weight { get; set; }

        [JsonProperty("measurements")]
        public string measurements { get; set; }

        [JsonProperty("tattoos")]
        public object tattoos { get; set; }

        [JsonProperty("piercings")]
        public object piercings { get; set; }

        [JsonProperty("yearsactive")]
        public object yearsactive { get; set; }

        [JsonProperty("cupsize")]
        public object cupsize { get; set; }

        [JsonProperty("fakeboobs")]
        public bool fakeboobs { get; set; }

        [JsonProperty("status")]
        public object status { get; set; }
    }

    public class Extras
    {

        [JsonProperty("gender")]
        public string gender { get; set; }

        [JsonProperty("birthday")]
        public string birthday { get; set; }

        [JsonProperty("birthday_timestamp")]
        public int birthday_timestamp { get; set; }

        [JsonProperty("birthplace")]
        public string birthplace { get; set; }

        [JsonProperty("birthplace_code")]
        public string birthplace_code { get; set; }

        [JsonProperty("active")]
        public int? active { get; set; }

        [JsonProperty("astrology")]
        public string astrology { get; set; }

        [JsonProperty("ethnicity")]
        public string ethnicity { get; set; }

        [JsonProperty("nationality")]
        public string nationality { get; set; }

        [JsonProperty("hair_colour")]
        public string hair_colour { get; set; }

        [JsonProperty("weight")]
        public string weight { get; set; }

        [JsonProperty("height")]
        public string height { get; set; }

        [JsonProperty("measurements")]
        public string measurements { get; set; }

        [JsonProperty("cupsize")]
        public string cupsize { get; set; }

        [JsonProperty("tattoos")]
        public string tattoos { get; set; }

        [JsonProperty("piercings")]
        public string piercings { get; set; }

        [JsonProperty("first_seen")]
        public DateTime first_seen { get; set; }

        [JsonProperty("waist")]
        public string waist { get; set; }

        [JsonProperty("hips")]
        public string hips { get; set; }
    }

    public class Poster
    {

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("size")]
        public int size { get; set; }

        [JsonProperty("order")]
        public int order { get; set; }
    }

    public class Parent
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("bio")]
        public string bio { get; set; }

        [JsonProperty("extras")]
        public Extras extras { get; set; }

        [JsonProperty("aliases")]
        public IList<object> aliases { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("thumbnail")]
        public string thumbnail { get; set; }

        [JsonProperty("posters")]
        public IList<Poster> posters { get; set; }
    }

    public class Performer
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("bio")]
        public string bio { get; set; }

        [JsonProperty("extra")]
        public Extra extra { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("thumbnail")]
        public string thumbnail { get; set; }

        [JsonProperty("parent")]
        public Parent parent { get; set; }
    }

    public class Site
    {

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("short_name")]
        public string short_name { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("logo")]
        public string logo { get; set; }

        [JsonProperty("favicon")]
        public string favicon { get; set; }
    }

    public class Tag
    {

        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("tag")]
        public string tag { get; set; }
    }

    public class Data
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("slug")]
        public string slug { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("site_id")]
        public int site_id { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("poster")]
        public string poster { get; set; }

        [JsonProperty("trailer")]
        public string trailer { get; set; }

        [JsonProperty("posters")]
        public Posters posters { get; set; }

        [JsonProperty("background")]
        public Background background { get; set; }

        [JsonProperty("created")]
        public string created { get; set; }

        [JsonProperty("last_updated")]
        public string last_updated { get; set; }

        [JsonProperty("performers")]
        public IList<Performer> performers { get; set; }

        [JsonProperty("site")]
        public Site site { get; set; }

        [JsonProperty("tags")]
        public IList<Tag> tags { get; set; }

        [JsonProperty("hashes")]
        public IList<object> hashes { get; set; }

        [JsonProperty("movie")]
        public object movie { get; set; }
    }

    public class GetSceneByID
    {

        [JsonProperty("data")]
        public Data data { get; set; }
    }
}
