Feature: Calculator
	Simple calculator for adding two numbers

@ui @Login
Scenario: Login user as User
	Given I navigate to application
	And I click the Login link
	And I enter username and password
		| UserName | Password |
		| admin    | password |
	When I click login
	Then I should see user is logged in to the application