using System;
using TeamControlium.Controlium;

namespace Internal.Tester
{
    //  This is a dummy Control, used to test the ControlBase class.  This control simply maps to any HTML element.
    public class ControlBaseTester : ControlBase
    {
        public ControlBaseTester(ObjectMappingDetails mapping)
        {
            SetRootElement(mapping);
        }

        public ControlBaseTester(string title)
        {
            SetRootElement(ObjectMap.Root_ByTitle_CaseSensitive.ResolveParameters(title));
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
                    throw new UnableToSetOrGetText(ObjectMap.TextEntryBox, $"Error finding, clearing or setting text [{value}] in [{RootElement.Mapping.FriendlyName}]", ex);
                }
            }
            get
            {
                try
                {
                    return FindElement(ObjectMap.TextEntryBox).GetAttribute("value");
                }
                catch (Exception ex)
                {
                    throw new UnableToSetOrGetText(ObjectMap.TextEntryBox, $"Error finding or getting text from textbox in [{RootElement.Mapping.FriendlyName}]", ex);
                }
            }
        }

        public class ObjectMap
        {
            public static ObjectMappingDetails Root_ByTitle_CaseSensitive => new ObjectMappingDetails(".//div[./input[@name='q' and @title='{0}']]", "Input Textbox with title [{0}] (Case sensitive)");

            public static ObjectMappingDetails TextEntryBox => new ObjectMappingDetails("./input[@name='q']", "Input Textbox text entry box");

            public static ObjectMappingDetails Div => new ObjectMappingDetails("./div[@class='gsfi']", "Input Textbox gsfi");

            public static ObjectMappingDetails Input1 => new ObjectMappingDetails("./input[@id='gs_taif0']", "Input Textbox taif");

            public static ObjectMappingDetails Input2 => new ObjectMappingDetails("./input[@id='gs_htif0']", "Input Textbox htif");
        }
    }
}