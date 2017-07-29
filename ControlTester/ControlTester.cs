using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TeamControlium.Framework;

namespace Internal.Tester
{
    public class ControlTester : ControlBase
    {
        //
        //  This is a dummy Control, used to test the ControlBase class.  This control simply maps to any HTML element.
        //
        //
        //
        public ControlTester(ObjectMappingDetails mapping)
        {
            SetRootElement(mapping);
        }

        public ControlTester(string title)
        {
            SetRootElement(ObjectMap.Root_ByText_CaseSensitive.ResolveParameters(title));
        }

        public string Text
        {
            set
            {
                try
                {
                    FindElement(ObjectMap.TextEntryBox).Clear().SetText(value);
                }
                catch (Exception ex)
                {
                    throw new TeamControlium.Framework.SeleniumExceptions.UnableToSetOrGetText(ObjectMap.TextEntryBox, $"Error finding, clearing or setting text [{value}] in [{RootElement.MappingDetails.FriendlyName}]",ex);
                }
            }
            get
            {
                try
                {
                    return FindElement(ObjectMap.TextEntryBox).GetAllText();
                }
                catch (Exception ex)
                {
                    throw new TeamControlium.Framework.SeleniumExceptions.UnableToSetOrGetText(ObjectMap.TextEntryBox, $"Error finding or getting text from textbox in [{RootElement.MappingDetails.FriendlyName}]", ex);
                }
            }
        }


        public class ObjectMap
        {
            public static ObjectMappingDetails Root_ByText_CaseSensitive => new ObjectMappingDetails(".//div[normalize-space(div[@id='fkbx-text'])='{0}']","FK Textbox with title [{0}] (Case sensitive)");
            public static ObjectMappingDetails TextEntryBox => new ObjectMappingDetails("./input[@name='q']", "FK Textbox text entry box");
        }
    }
}
