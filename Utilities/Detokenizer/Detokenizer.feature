@Utilities_Detokenizer
Feature: Utilities_Detokenizer
	In order to process tokens
	As a test automator
	I want to be able to convert tokens to real texts

	#
	# Basic dates
	#

    Scenario: Convert today's date token to the date we run this test
	Given I have a string "{date;today;dd/MM/yyyy}"
	When I process the token
	Then the string is today's date in the format "dd/MM/yyyy"

	Scenario: Convert yesterday's date token to the day before we run this test
	Given I have a string "{date;yesterday;dd/MM/yyyy}"
	When I process the token
	Then the string is yesterday's date in the format "dd/MM/yyyy"

	Scenario: Convert tomorrows's date token to the day after we run this test
	Given I have a string "{date;tomorrow;dd/MM/yyyy}"
	When I process the token
	Then the string is tomorrows's date in the format "dd/MM/yyyy"

	Scenario: Date command and verb not case sensitive
	Given I have a string "{Date;Today;dd/MM/yyyy}"
	When I process the token
	Then the string is today's date in the format "dd/MM/yyyy"




	#
	# Output formatting
	#

	Scenario: Get the day of the week when test is run
	Given I have a string "{date;today;dddd}"
	When I process the token
	Then the string is today's date in the format "dddd"

	#
	# Complex offsets
	#
    Scenario: Convert date for a days offset in the past
	Given I have a string "{date;AddDays(-10);dd/MM/yyyy}"
	When I process the token
	Then the string is the date -10 "days" in the format "dd/MM/yyyy"

	Scenario: Convert date for a days offset in the future
	Given I have a string "{date;AddDays(10);dd/MM/yyyy}"
	When I process the token
	Then the string is the date 10 "days" in the format "dd/MM/yyyy"

    Scenario: Convert date for a days offset in the future (with a plus sign)
	Given I have a string "{date;AddDays(+10);dd/MM/yyyy}"
	When I process the token
	Then the string is the date 10 "days" in the format "dd/MM/yyyy"

    Scenario: Convert date for a months offset in the past
	Given I have a string "{date;AddMonths(-19);dd/MM/yyyy}"
	When I process the token
	Then the string is the date -19 "months" in the format "dd/MM/yyyy"

	Scenario: Convert date for a months offset in the future
	Given I have a string "{date;AddMonths(17);dd/MM/yyyy}"
	When I process the token
	Then the string is the date 17 "months" in the format "dd/MM/yyyy"

	Scenario: Convert date for a years offset in the past
	Given I have a string "{date;AddYears(-25);dd/MM/yyyy}"
	When I process the token
	Then the string is the date -25 "years" in the format "dd/MM/yyyy"

	Scenario: Convert date for a years offset in the future
	Given I have a string "{date;AddYears(25);dd/MM/yyyy}"
	When I process the token
	Then the string is the date 25 "years" in the format "dd/MM/yyyy"
