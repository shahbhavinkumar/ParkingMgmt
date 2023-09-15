# Parking Management
```
CREATE TABLE [dbo].[ParkingInformation](
	[TagNumber] [varchar](100) NOT NULL,
	[InTime] [datetime] NOT NULL,
	[OutTime] [datetime] NULL,
	[Rate] [decimal](10, 2) NULL
) ON [PRIMARY]
GO
```

===================================================================================

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
```
END
GO


