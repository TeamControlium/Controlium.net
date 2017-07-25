Feature: SeleniumDriverTests


Scenario Outline: Selenium can be launched with mandatory settings set
Given There are no processes running named <Server process name>
And There are no processes running named <Process name>
And setting Category "Selenium", Option "Browser" is <Browser>
And setting Category "Selenium", Option "Host" is "localhost"
And setting Category "Selenium", Option "SeleniumServerFolder" is ".//..//..//TestSeleniumServer"
And setting Category "Selenium", Option "DebugMode" is "off"
When I instantiate SeleniumDriver
Then  a process exists named <Server process name>
And a process exists named <Process name>
Examples:
| Browser  | Process name | Server process name |
| "IE11"   | "iexplore"   | "IEDriverServer"    |
| "Chrome" | "chrome"     | "chromedriver"      |

Scenario Outline: When I use SeleniumDriver to browse to Google Search I get the correct page title
Given There are no processes running named <Server process name>
And There are no processes running named <Process name>
And setting Category "Selenium", Option "Browser" is <Browser>
And setting Category "Selenium", Option "Host" is "localhost"
And setting Category "Selenium", Option "SeleniumServerFolder" is ".//..//..//TestSeleniumServer"
And setting Category "Selenium", Option "DebugMode" is "off"
And I instantiate SeleniumDriver
When I browse to "http://www.google.com"
Then I can read the page title "Google"
Examples:
| Browser  | Process name | Server process name |
| "IE11"   | "iexplore"   | "IEDriverServer"    |
| "Chrome" | "chrome"     | "chromedriver"      |


Scenario Outline: I can find an element on the page from the top level
Given There are no processes running named <Server process name>
And There are no processes running named <Process name>
And setting Category "Selenium", Option "Browser" is <Browser>
And setting Category "Selenium", Option "Host" is "localhost"
And setting Category "Selenium", Option "SeleniumServerFolder" is ".//..//..//TestSeleniumServer"
And setting Category "Selenium", Option "DebugMode" is "off"
And I instantiate SeleniumDriver
And I browse to "http://www.google.com"
When I use FindElement to locate an element using XPath "//input[@name='q']"
Then a valid element is found
Examples:
| Browser  | Process name | Server process name |
| "IE11"   | "iexplore"   | "IEDriverServer"    |
| "Chrome" | "chrome"     | "chromedriver"      |

