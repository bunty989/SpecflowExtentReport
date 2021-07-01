Feature: Login
	Check if login functionality works


@ui @Login
Scenario: Login user as Administrator
	Given I navigate to application
	And I click the Login link
	And I enter username and password
		| UserName | Password |
		| admin    | password |
	When I click login
	Then I should see user is logged in to the application