Feature: ControlBase
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario Outline: I can find a Control from the top level
Given I am using local browser "Chrome"
And I instantiate SeleniumDriver and browse to "http://www.google.com"
When I find FK Textbox control with title "Search Google or type URL"
Then a valid element is found
Examples:
| Browser  | Process name | Server process name |
| "IE11"   | "iexplore"   | "IEDriverServer"    |
| "Chrome" | "chrome"     | "chromedriver"      |

