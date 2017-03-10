Feature: Logger features
	In order to test log events in the Controlium solution
	As a test automator
	I want to be able to log events

Scenario: When Logger is set to Verbose all categories of write are written to output
	Given I have configured Logger to write to a string
	And I set Logger to level "Maximum Output"
	When I call Logger with level "hello" and string "wibble"
	Then the string written to by Logger should end with "wibble"
