using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGlass {
	public class IRDevice {
		public string device_id;
		public string device_type;
		public string device_brand;
		public string device_model;
		[JsonProperty("buttons")]
		public Dictionary<string, string> button_to_standard_bind;
	}
}
