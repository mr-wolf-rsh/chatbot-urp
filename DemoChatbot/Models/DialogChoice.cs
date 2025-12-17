using System.Collections.Generic;

namespace DemoChatbot.Models
{
    public class DialogChoice
    {
        public string DialogId { get; set; }

        public List<string> ChoiceList { get; set; }
    }
}
