Feature: SeleniumDriverTests

Background: 
    Given There are no processes running named "IEDriverServer"
    And There are no processes running named "iexplore"
    And There are no processes running named "chromedriver"
    And There are no processes running named "chrome"
    And setting Category "Selenium", Option "Host" is "localhost"
    And setting Category "Selenium", Option "SeleniumServerFolder" is ".//..//..//TestSeleniumServer"
    And setting Category "Selenium", Option "DebugMode" is "off"

Scenario Outline: Selenium can be launched with mandatory settings set
    Given setting Category "Selenium", Option "Browser" is <Browser>
    When I instantiate SeleniumDriver
    Then  a process exists named <Server process name>
    And a process exists named <Process name>

Examples:
    | Browser  | Process name | Server process name |
    | "IE11"   | "iexplore"   | "IEDriverServer"    |
    | "Chrome" | "chrome"     | "chromedriver"      |

Scenario Outline: When I use SeleniumDriver to browse to Google Search I get the correct page title
    Given Setting Category "Selenium", Option "Browser" is <Browser>
    And I instantiate SeleniumDriver
    When I browse to "http://www.google.com"
    Then I can read the page title "Google"

Examples:
    | Browser  |
    | "IE11"   |
    | "Chrome" |

Scenario Outline: I can find an element on the page from the top level
    Given setting Category "Selenium", Option "Browser" is <Browser>
    And I instantiate SeleniumDriver
    And I browse to "http://www.google.com"
    When I use FindElement to locate an element using XPath "//input[@name='q']"
    Then a valid element is found

Examples:
    | Browser  |
    | "IE11"   |
    | "Chrome" |
