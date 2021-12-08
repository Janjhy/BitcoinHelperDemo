# BitcoinHelperDemoSolution
This project is an API to handle data from the Crypto API. It uses the ASP.NET framework for the main project and additionally xUnit for unit testing.


## Running 
This project requires .NET SDK 6.0.

Clone this repo to your computer. 
Navigate to BitcoinHelperDemo folder and run the project with "dotnet run" command.

The project base url will be http://localhost:5138. This is modifiable in the launch settings.

## Routes

GET: /BitcoinHelper/{param}/{YYYY}/{mm}/{dd}/to/{YYYY}/{mm}/{dd}

The first "{YYYY}/{mm}/{dd}" is the start of the date range, and after to/ is the end for the date range.  

{YYYY} is a year value such as 2020.  
{mm} is a month value such as 2, for February.  
{dd} is day value such as 30.  

The available param values are: 

### downwardtrend: 
If valid date range is given, the route returns the longest streak of days as a number, where the price of bitcoin decreased.   
If prices dont decrease in given range, the "Text" field indicates so, and the consecutive days value is zero.

Example request: "BitcoinHelper/downwardtrend/2020/03/1/to/2021/8/01"  

Returns:   
{"ConsecutiveDecreaseDays":8,"Text":null,"From":"2020-03-01T00:00:00+00:00","To":"2021-08-01T00:00:00+00:00"}  


### tradinghigh:
If valid date range is given, the route returns the day and the value of the highest trading day by volume.  

Example request:  "BitcoinHelper/tradinghigh/2020/03/1/to/2021/8/01"  

Returns:  
{"Day":"2021-01-04T00:00:00+00:00","TradingVolume":146032480261.85092,"From":"2020-03-01T00:00:00+00:00","To":"2021-08-01T00:00:00+00:00"}  


### timemachine:
If valid date range is given, the route calculates the best pair of days when to buy and sell for maximum profit.
If no profit can be made in the given range, the "Text" field indicates so, and the "BuyDate" and "SellDate" values are null.

Example request:  "/BitcoinHelper/timemachine/2020/01/1/to/2020/3/01"   

Returns:  
{"BuyDate":"2020-01-03T00:02:33.859+00:00","SellDate":"2020-02-15T00:04:03.292+00:00","Text":null,"From":"2020-01-01T00:00:00+00:00","To":"2020-03-01T00:00:00+00:00"} 
 
 
 Note that the dates in the responses are in extended ISO 8601 format. 
