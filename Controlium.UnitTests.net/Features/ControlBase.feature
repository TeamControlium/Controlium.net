Feature: ControlBase

Scenario Outline: I can find a Control from the top level
    Given I am using local browser <Browser>
    And I instantiate SeleniumDriver and browse to "http://www.google.com"
    When I find Textbox control with title "Search"
    Then Control is found

Examples:
    | Browser  | 
    | "Edge"   |
    | "Chrome" |


Scenario Outline: I can use a method within a found control
    Given I am using local browser <Browser>
    And I instantiate SeleniumDriver and browse to "http://www.google.com"
    When I find Textbox control with title "Search"
    And enter text "this is a search"
    Then I can read the text "this is a search"

Examples:
    | Browser  |
    | "Edge"   |
    | "Chrome" |
