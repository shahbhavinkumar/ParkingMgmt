USE [ParkingManagement]
GO

/****** Object:  Table [dbo].[ParkingInformation]    Script Date: 9/15/2023 2:43:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ParkingInformation](
	[TagNumber] [varchar](100) NOT NULL,
	[InTime] [datetime] NOT NULL,
	[OutTime] [datetime] NULL,
	[Rate] [decimal](10, 2) NULL
) ON [PRIMARY]
GO


