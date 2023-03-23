# CompanyClaims

My Solution for the API Tech Test.

## Notes
Solution is using .Net 7 - minimal APIs.

The Repository is just a dictionary - with hindsight I think it would have been better, and easier to do it with a SqlLite backend or something and an ORM like Dapper.
I've tried to generate random data.

I ignored the ClaimType table in the problem spec - as I wasn't 100% sure what it was for.

As the problem specification is very light on business logic - I've struggled to do any meaning full unit tests.  I've added some unit tests to check the repositories are working and I've done some to validate the mapping between entities and the dtos.

