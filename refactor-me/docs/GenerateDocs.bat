cd %~dp0
call npm i
call ./node_modules/.bin/raml2html api.raml > index.html
