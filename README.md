# BitcoinHelperDemoSolution

## Running 

## Routes

GET: /BitcoinHelper/{param}/{YYYY}/{mm}/{dd}/to/{YYYY}/{mm}/{dd}

The first "{YYYY}/{mm}/{dd}" is the start of the date range, and after to/ is the end for the date range.  

{YYYY} is a year value such as 2020.  
{mm} is a month value such as 2, for February.  
{dd} is day value such as 30.  

The available param values are: 

### downwardtrend: 
If valid date range is given, the route returns the longest streak of days as a number, where the price of bitcoin decreased.   

Example request: "BitcoinHelper/downwardtrend/2020/03/1/to/2021/8/01"  

Returns {"ConsecutiveDecreaseDays":8,"Text":null,"From":"2020-03-01T00:00:00+00:00","To":"2021-08-01T00:00:00+00:00"}  


### tradinghigh:
If valid date range is given, the route returns the day and the value of the highest trading day by volume.  

Example request:  "BitcoinHelper/tradinghigh/2020/03/1/to/2021/8/01"  

Returns {"Day":"2021-01-04T00:00:00+00:00","TradingVolume":146032480261.85092,"From":"2020-03-01T00:00:00+00:00","To":"2021-08-01T00:00:00+00:00"}  

### timemachine:
If valid date range is given, the route calculates the best pair of days when to buy and sell for maximum profit.

Example request:  "/BitcoinHelper/timemachine/2020/01/1/to/2020/3/01"   

Returns {"BuyDate":"2020-01-03T02:05:40.219+00:00","SellDate":"2020-01-03T12:02:36.99+00:00","Text":null,"From":"2020-01-01T00:00:00+00:00","To":"2020-03-01T00:00:00+00:00"}  
 
