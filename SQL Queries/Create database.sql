USE [master]
GO

/****** Object:  Database [Anoroc]    Script Date: 2020/07/15 11:06:22 AM ******/
CREATE DATABASE [Anoroc]
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Anoroc].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [Anoroc] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Anoroc] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Anoroc] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Anoroc] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Anoroc] SET ARITHABORT OFF 
GO

ALTER DATABASE [Anoroc] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Anoroc] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Anoroc] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Anoroc] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Anoroc] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Anoroc] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Anoroc] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Anoroc] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Anoroc] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Anoroc] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Anoroc] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Anoroc] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Anoroc] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Anoroc] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Anoroc] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Anoroc] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Anoroc] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Anoroc] SET RECOVERY FULL 
GO

ALTER DATABASE [Anoroc] SET  MULTI_USER 
GO

ALTER DATABASE [Anoroc] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Anoroc] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Anoroc] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Anoroc] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Anoroc] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Anoroc] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Anoroc] SET  READ_WRITE 
GO

USE [Anoroc]
GO

/****** Object:  Table [dbo].[Areas]    Script Date: 2020/07/15 11:07:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Areas](
	[Area_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Country] [varchar](255) NOT NULL,
	[Province] [varchar](255) NOT NULL,
	[Suburb] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Area_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


USE [Anoroc]
GO

/****** Object:  Table [dbo].[Clusters]    Script Date: 2020/07/15 11:07:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Clusters](
	[Cluster_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Coordinates] [nvarchar](max) NOT NULL,
	[Center_Location] [nvarchar](max) NOT NULL,
	[Carrier_Data_Points] [int] NOT NULL,
	[Cluster_Created] [datetime] NOT NULL,
	[Cluster_Radius] [decimal](18, 0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Cluster_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [Anoroc]
GO

/****** Object:  Table [dbo].[Locations]    Script Date: 2020/07/15 11:08:11 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Locations](
	[Location_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Coordinate] [nvarchar](max) NOT NULL,
	[Carrier_Data_Point] [bit] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Region] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Location_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users](
	[User_ID] [bigint] IDENTITY(1,1) NOT NULL,
	[First_Name] [nvarchar](max) NOT NULL,
	[Last_Name] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Access_Token] [nvarchar](max) NOT NULL,
	[Facebook_Log_In] [bit] NOT NULL,
	[Google_Log_In] [bit] NOT NULL,
	[Anoroc_Log_In] [bit] NOT NULL,
	[Carrier_Status] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


