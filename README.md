# Parking Management

Constraints:

• Backend technology: 
	o WebApp: .net core
▪ The pages must be server-side-rendered. You can utilize MVC or razor pages
     type projects.
▪ Preferably, do not use Entity framework.
	o Database: SQL server

• Frontend: 
	o plain JavaScript and/or typescript. 
▪ Any JavaScript ES version can be used as long it’s supported by the latest 
production version of the chromium engine.
▪ No Front-End JavaScript frameworks

• The total number of spots and hourly fee need to be defined in the appsettings.json file.

-------------------------------- Use Case -----------------------------
Cars coming in:
Whenever a new car arrives, the operator types the tag number and clicks 'In'. The app needs to validate 
the following.
* Any spots available?
* Is the car already in the parking lot?
Whenever the validation fails, the operator needs to be given an error message.
Whenever the validation passes, 'Area B' needs to be updated via ajax request. (no full page reload)
Cars coming out:
Whenever a car leaves the parking lot, the operator types the tag number and clicks 'Out'. The app 
needs to validate the following.
* Is the car registered in the parking lot
Whenever the validation fails, the operator needs to be given an error message


Stats
It opens a modal with the following information.
• Number of spots available as of now
• Today’s revenue as of now
• Average number of cars per day (for the past 30 days)


-------------------------------- Scripts -----------------------------

```
CREATE TABLE [dbo].[ParkingInformation](
	[TagNumber] [varchar](100) NOT NULL,
	[InTime] [datetime] NOT NULL,
	[OutTime] [datetime] NULL,
	[Rate] [decimal](10, 2) NULL
) ON [PRIMARY]
GO
```


```
CREATE   PROCEDURE [dbo].[sp_GetReportData]
AS
BEGIN

DECLARE @TodaysRevenue DECIMAL(10, 2) 

CREATE TABLE #temp
(
	TodaysRevenue DECIMAL(10, 2)  NULL,
	AvgRevenueThirtyDays VARCHAR(50) NULL,
	AvgCarsThirtyDays int NULL
)


SELECT @TodaysRevenue = SUM(Rate)
FROM ParkingInformation
WHERE CAST(InTime AS DATE) = CAST(GETDATE() AS DATE)
 AND OutTime is Not Null;

INSERT INTO #temp (TodaysRevenue, AvgCarsThirtyDays, AvgRevenueThirtyDays) 
SELECT @TodaysRevenue, AVG(CarCount) AS AvgCarsPerday, AVG(DailyRevenue) AS averageRevenuePerDay
FROM (
    SELECT COUNT(*) AS CarCount, CAST(InTime AS DATE) AS DateofParking, SUM(Rate) AS DailyRevenue 
    FROM ParkingInformation
    WHERE InTime >= DATEADD(DAY, -30, GETDATE()) AND  OutTime is Not Null
    GROUP BY CAST(InTime AS DATE)
) s;

SELECT * FROM #temp

drop table #temp

END
GO
```

