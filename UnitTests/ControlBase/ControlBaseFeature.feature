Feature: ControlBase

Background: 
    Given I am using local browser "Chrome"
    And I instantiate SeleniumDriver and browse to "http://www.google.com"

Scenario Outline: I can find a Control from the top level
    When I find Textbox control with title "Search"
    Then Control is found

Examples:
    | Browser  | Process name | Server process name |
    | "IE11"   | "iexplore"   | "IEDriverServer"    |
    | "Chrome" | "chrome"     | "chromedriver"      |


Scenario Outline: I can use a method within a found control
    When I find Textbox control with title "Search"
    And enter text "this is a search"
    Then I can read the text "this is a search"

Examples:
    | Browser  | Process name | Server process name |
    | "IE11"   | "iexplore"   | "IEDriverServer"    |
    | "Chrome" | "chrome"     | "chromedriver"      |
