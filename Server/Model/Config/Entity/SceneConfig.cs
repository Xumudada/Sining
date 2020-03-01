using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sining.Config
{
	public partial class SceneConfig : IConfig, IBson
	{
		public int Id { get; set; }
		[BsonDefaultValue("")]
		public string SceneType { get; set; }
		[BsonDefaultValue(0)]
		public int Server { get; set; }
		[BsonDefaultValue(0)]
		public int Zone { get; set; }
		[BsonDefaultValue(0)]
		public int OuterPort { get; set; }
		[BsonDefaultValue("")]
		public string NetworkProtocol { get; set; }
	}
}