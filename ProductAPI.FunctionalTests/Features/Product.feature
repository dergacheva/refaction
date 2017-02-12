Feature: Product
	In order to manipulate products data
	As an API consumer
	I want to perform CRUD operations on product API

Scenario: Create a new product
	Given an http request with "POST" verb and "CreatedProduct" body
	When I call "Products" endpoint with "no" query
	Then I get NoContent response

Scenario: Get list of products
	Given I have created a new product
	Given an http request with "GET" verb and "no" body
	When I call "Products" endpoint with "no" query
	Then the result should include product items
    And I get OK response

Scenario: Get list of products filtered by name
	Given I have created a new product
	Given an http request with "GET" verb and "no" body
	When I call "Products" endpoint with "name" query
	Then the result should include product items
    And I get OK response

Scenario: Get product by id
	Given I have created a new product
	Given an http request with "GET" verb and "no" body
	When I call "ProductsById" endpoint with "no" query
	Then the result should include product
    And I get OK response

Scenario: Update product by id
	Given I have created a new product
	Given an http request with "PUT" verb and "UpdatedProduct" body
	When I call "ProductsById" endpoint with "no" query
	Then I get NoContent response

Scenario: Delete product
	Given I have created a new product
	Given an http request with "DELETE" verb and "no" body
	When I call "ProductsById" endpoint with "no" query
	Then I get NoContent response

Scenario: Create a new option
	Given I have created a new product
	Given an http request with "POST" verb and "CreatedOption" body
	When I call "Options" endpoint with "no" query
	Then I get NoContent response

Scenario: Get list of product options
	Given I have created a new product
	Given I have created a new product option
	Given an http request with "GET" verb and "no" body
	When I call "Options" endpoint with "no" query
	Then the result should include product options item
    And I get OK response

Scenario: Get product option by id
	Given I have created a new product
	Given I have created a new product option
	Given an http request with "GET" verb and "no" body
	When I call "OptionsById" endpoint with "no" query
	Then the result should include product option
    And I get OK response

Scenario: Update product option by id
	Given I have created a new product
	Given I have created a new product option
	Given an http request with "PUT" verb and "UpdatedOption" body
	When I call "OptionsById" endpoint with "no" query
	Then I get NoContent response

Scenario: Delete product option
	Given I have created a new product
	Given I have created a new product option
	Given an http request with "DELETE" verb and "no" body
	When I call "OptionsById" endpoint with "no" query
	Then I get NoContent response
