USE [master]
GO
/****** Object:  Database [menfs]    Script Date: 06/03/2024 5:54:55 PM ******/
CREATE DATABASE [menfs]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'menfs', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\menfs.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'menfs_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\menfs_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [menfs] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [menfs].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [menfs] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [menfs] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [menfs] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [menfs] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [menfs] SET ARITHABORT OFF 
GO
ALTER DATABASE [menfs] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [menfs] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [menfs] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [menfs] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [menfs] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [menfs] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [menfs] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [menfs] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [menfs] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [menfs] SET  DISABLE_BROKER 
GO
ALTER DATABASE [menfs] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [menfs] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [menfs] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [menfs] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [menfs] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [menfs] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [menfs] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [menfs] SET RECOVERY FULL 
GO
ALTER DATABASE [menfs] SET  MULTI_USER 
GO
ALTER DATABASE [menfs] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [menfs] SET DB_CHAINING OFF 
GO
ALTER DATABASE [menfs] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [menfs] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [menfs] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [menfs] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [menfs] SET QUERY_STORE = OFF
GO
USE [menfs]
GO
/****** Object:  Table [dbo].[Invoince]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoince](
	[invoinceId] [uniqueidentifier] NOT NULL,
	[deliveryStatus] [bit] NULL,
	[deliveryDate] [smalldatetime] NULL,
	[totalMoney] [int] NOT NULL,
	[memberId] [uniqueidentifier] NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[dateCreate] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[invoinceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vDoanhThuTheoNgay]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vDoanhThuTheoNgay] AS
SELECT ISNULL(CONVERT(VARCHAR(10), dateCreate, 103),-1) AS dateCreate, sum(totalMoney) AS income
FROM dbo.Invoince hd 
GROUP BY CONVERT(VARCHAR(10), dateCreate, 103)
GO
/****** Object:  View [dbo].[vHoaDonTrongNgay]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vHoaDonTrongNgay] AS
select *
from dbo.Invoince
where DATEPART(day,dateCreate) = DATEPART(day,getdate())
GO
/****** Object:  Table [dbo].[Article]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Article](
	[articleId] [uniqueidentifier] NOT NULL,
	[title] [nvarchar](250) NULL,
	[description] [nvarchar](2000) NULL,
	[image] [nvarchar](2000) NULL,
	[content] [nvarchar](max) NULL,
	[memberId] [uniqueidentifier] NOT NULL,
	[meta] [nvarchar](500) NULL,
	[status] [bit] NULL,
	[dateCreate] [smalldatetime] NULL,
 CONSTRAINT [PK__Article__75D3D37E1E54D497] PRIMARY KEY CLUSTERED 
(
	[articleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Contact]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact](
	[contactId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](250) NULL,
	[email] [varchar](50) NULL,
	[message] [nvarchar](2000) NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[dateContact] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[contactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InvoinceDetail]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoinceDetail](
	[invoinceId] [uniqueidentifier] NOT NULL,
	[productId] [uniqueidentifier] NOT NULL,
	[size] [varchar](10) NULL,
	[quanlity] [int] NULL,
	[price] [int] NULL,
	[discount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[invoinceId] ASC,
	[productId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[memberId] [uniqueidentifier] NOT NULL,
	[phone] [varchar](20) NULL,
	[password] [varchar](50) NULL,
	[firstName] [nvarchar](250) NULL,
	[lastName] [nvarchar](250) NULL,
	[email] [varchar](50) NULL,
	[birthday] [date] NULL,
	[address] [nvarchar](250) NULL,
	[avatar] [nvarchar](2000) NULL,
	[roleId] [uniqueidentifier] NOT NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[dateCreate] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[memberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[menuId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](255) NULL,
	[meta] [varchar](255) NULL,
	[status] [bit] NULL,
	[order] [int] NULL,
	[dateCreate] [smalldatetime] NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[menuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[productId] [uniqueidentifier] NOT NULL,
	[productName] [nvarchar](250) NULL,
	[image] [varchar](2000) NULL,
	[price] [int] NULL,
	[discount] [int] NULL,
	[characteristic] [nvarchar](200) NULL,
	[description] [nvarchar](2000) NULL,
	[quanlity] [int] NULL,
	[brand] [nvarchar](250) NULL,
	[categoryId] [uniqueidentifier] NOT NULL,
	[memberId] [uniqueidentifier] NOT NULL,
	[meta] [nvarchar](250) NULL,
	[status] [bit] NULL,
	[dateCreate] [smalldatetime] NULL,
 CONSTRAINT [PK__Product__2D10D16ACF41DC17] PRIMARY KEY CLUSTERED 
(
	[productId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[categoryId] [uniqueidentifier] NOT NULL,
	[categoryName] [nvarchar](250) NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[order] [int] NULL,
	[dateCreate] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[categoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[roleId] [uniqueidentifier] NOT NULL,
	[roleName] [nvarchar](50) NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[dateCreate] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[roleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Slide]    Script Date: 06/03/2024 5:54:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Slide](
	[slideId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](50) NULL,
	[description] [nvarchar](250) NULL,
	[url] [varchar](50) NULL,
	[meta] [nvarchar](50) NULL,
	[status] [bit] NULL,
	[order] [int] NULL,
	[dateCreate] [smalldatetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[slideId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'421b280c-d5a9-4072-a0da-028f8cdb8355', N'9 Gym Shoes That Will Take Your Workouts To The Next Level', N'', N'~/Content/img/blog/Nike-Metcon-7-1160x677.jpg', N'<p>Not all&nbsp;<a href="https://www.apetogentleman.com/best-sneaker-brands/" data-wpel-link="internal">sneakers</a>&nbsp;are born equal. That goes for your casual kicks as much as it does for the average workout shoe. With moon landing levels of tech being integrated into ever-more impressive trainers, it&rsquo;s difficult to sort through the gimmicks and the foam soles.</p>
<p>But worry not, we desire that perfect athletic body as much as the next man &ndash; and we&rsquo;re here to guide you through the best possible footwear to achieve your goals, whether it&rsquo;s lifting personal bests, sweating through HIIT workouts or tearing up a CrossFit circuit.</p>
<h2>Purchasing Considerations</h2>
<p>Before parting with your hard-earned cash, you should consider a couple of things. Firstly, like with clothing, fit is king. You&rsquo;re not going to get the most out of your workout if you&rsquo;re wearing a shoe that doesn&rsquo;t fit properly.</p>
<p>Shoe sizes that work in different corridors of your life might not be appropriate in the workout or lifting arena. For example, if you&rsquo;re lifting heavy weight you&rsquo;ll want a wider shoe to ensure a more stable base and help transfer power. Likewise, you don&rsquo;t want to do CrossFit in a shoe that is too small and doesn&rsquo;t allow manoeuvrability, or pound out cardio in sneakers that aren&rsquo;t supremely comfortable and light.</p>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/gym-shoes.jpg" /></p>
<p>Secondly, you may need different shoes for different activities. There are sneakers on this list that are specifically for weightlifting sessions &ndash; wide bases, stiffer materials and ankle support built in as well as a tell-tale toe strap are the giveaways. There&rsquo;s also all-rounders, with support in the mid-sole and added cushioning for extended periods of movement. Lighter shoes, meanwhile, will allow for better bursts of explosive power in HIIT workouts. If you like to mix up your workouts, we&rsquo;d recommend having a rotation of shoes that can be called on.</p>
<p>With that in mind here&rsquo;s our selection of the best gym trainers with options for different workouts, styles and budgets.</p>
<h2>Nike Metcon 7</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/gymtrainersone.jpg" /></p>
<p>The latest iteration of a classic, Nike&rsquo;s Metcon 7 is a great-looking sneaker. The tech and CrossFit input into this model is astounding and includes integrated heel support to stabilise the force on different areas of the shoe when lifting, an inner plate that spreads the weight from edge to edge, rubber added to the sides for more traction on rope climbs, react foam for comfort and added spring on sprints, as well as a tab that locks down your laces so you don&rsquo;t ever have to worry about them coming untied.</p>
<p>Stiffness and lightness come as standard, with neither sacrificed for the other. A true all-rounder.</p>
<h2>On Cloud X</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/gymtrainers2.jpg" /></p>
<p>Whilst On is very much a&nbsp;<a href="https://www.apetogentleman.com/best-running-shoes-brands/" data-wpel-link="internal">running shoe brand</a>, with the Swiss company managing to break into an arena that was previously reserved for&nbsp;<a href="https://www.apetogentleman.com/best-sportswear-brands/" data-wpel-link="internal">sportswear giants</a>, the Cloud X is a sneaker that can hold its own across the gym floor.</p>
<p>Cloudtec soles keep things comfortably bouncy when you need to dig in and keep bursting forward, while the dual-density sock liner and knit-weave upper both work together to ensure comfort and wick away sweat. They&rsquo;re also extremely light without feeling like they&rsquo;re going to fall apart.</p>
<h2>Reebok Nano X1 Adventure</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/gymtrainers3.jpg" /></p>
<p>Sturdy and robust without feeling like every step is a rep, Reebok&rsquo;s Nano X1 Adventure sneakers are for those who prefer their workouts outdoors.</p>
<p>Engineered to be used on differing terrain &ndash; be it concrete, grass or even sand &ndash; the sole is reinforced and features raised lugs for extra grip while the Floatride Energy Foam ensures comfort over long periods and a responsive feel that is imperative when dealing with uneven surfaces.</p>
<div id="apeto-3c6c6fe54e0f39d7e2d7826fc35af6ef" class="apeto-3c6c6fe54e0f39d7e2d7826fc35af6ef apeto-in-content-ad-after-intro">
<div id="apeto-14415451" class="apeto-in-content-ad-after-intro"></div>
</div>', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'9-Gym-Shoes-That-Will-Take-Your-Workouts-To-The-Next-Level', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'2ed5ed8c-e77f-4346-a719-034a5c771173', N'Perfect Pairings: What Shoes To Wear With Jeans', N'', N'~/Content/img/blog/what-shoes-with-jeans-1-1160x676.jpg', N'<p>Jeans have been the world&rsquo;s favourite legwear for just shy of 100 years. Why? Probably because they&rsquo;re famously durable,&nbsp;<a href="https://www.apetogentleman.com/what-to-wear-denim-jeans-colours/" data-wpel-link="internal">easy to wear</a>&nbsp;and versatile to the extreme. But while they&rsquo;re generally straightforward to style, there are certain pairing guidelines to follow if you want&nbsp;<a href="https://www.apetogentleman.com/best-denim-jeans-brands-men/" data-wpel-link="internal">your jeans to look their best</a>&hellip; particularly when it comes to footwear.</p>
<p>So what shoes should you wear with your jeans? It&rsquo;s a question that every style-conscious man has asked himself at some point. As versatile as jeans are, there are certain&nbsp;<a href="https://www.apetogentleman.com/types-of-shoes/" data-wpel-link="internal">types of shoes</a>&nbsp;that will never look good with&nbsp;<a href="https://www.apetogentleman.com/what-to-wear-denim-jeans-colours/" data-wpel-link="internal">specific shades of denim</a>, and others that will look great every time.</p>
<p>The key to nailing it is knowing what those shades and styles are. So, to help you get it right, we&rsquo;ve prepared a handy cheat sheet, highlighting five key denim shades and the types of footwear that work best with each one.</p>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2020/12/howtowearleather5.jpg" /></p>
<p>Jeans don&rsquo;t typically work well in smart settings &ndash; unless they&rsquo;re black, that is. A&nbsp;<a href="https://www.apetogentleman.com/what-to-wear-black-jeans-men/" data-wpel-link="internal">pair of black jeans</a>&nbsp;is sharp, subtle and perfect for occasions that call for clothing on the dressier end of the&nbsp;<a href="https://www.apetogentleman.com/smart-casual/" data-wpel-link="internal">smart-casual spectrum</a>, such as dates, upmarket restaurants and bars, and nights out.</p>
<p>To make the most of their aesthetic benefits, you&rsquo;ll want to make sure you choose your shoes carefully. As a rule of thumb, it&rsquo;s best to stick to dark colours &ndash; you&rsquo;re aiming for subtle, not jarring. Colours like burgundy, dark brown, dark green and, of course, black, will all work nicely. An exception to this rule is&nbsp;<a href="https://www.apetogentleman.com/best-white-sneakers-men/" data-wpel-link="internal">white minimalist sneakers</a>, which will always perfectly complement slim-cut black jeans.</p>
<p>Keep the style of shoe relatively smart too. Think&nbsp;<a href="https://www.apetogentleman.com/best-chelsea-boots-men/" data-wpel-link="internal">Chelsea boots</a>,&nbsp;<a href="https://www.apetogentleman.com/best-chukka-desert-boots/" data-wpel-link="internal">desert boots</a>,&nbsp;<a href="https://www.apetogentleman.com/best-luxury-sneaker-brands/" data-wpel-link="internal">luxe sneakers</a>&nbsp;and Derby shoes. A nice&nbsp;<a href="https://www.apetogentleman.com/best-loafers-men/" data-wpel-link="internal">loafer</a>&nbsp;or deck shoe will work well in the warmer months too.</p>
<h2>What Shoes To Wear With Dark Jeans</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/09/shoeswithjeansraw3.jpg" /></p>
<p>A pair of proper&nbsp;<a href="https://www.apetogentleman.com/best-raw-selvedge-denim-jeans/" data-wpel-link="internal">raw-denim jeans</a>&nbsp;is something that every&nbsp;<a href="https://www.apetogentleman.com/how-to-dress-well-rules/" data-wpel-link="internal">well-dressed man</a>&nbsp;should have hanging proudly in his wardrobe. This is denim in its purest form: heavy, unwashed and rugged. It&rsquo;s most at home with casual, smart-casual and&nbsp;<a href="https://www.apetogentleman.com/best-workwear-brands/" data-wpel-link="internal">workwear-based garments</a>&nbsp;and that extends to the footwear it should be paired with it too.</p>
<p>Raw denim is dark, so you should approach colour matching your shoes much the same as you would with black jeans. We&rsquo;re talking dark colours &ndash; black, brown, burgundy, etc &ndash; with the notable exception of sneakers, where there&rsquo;s a little more room for manoeuvre. Just steer clear of navy and very light browns, like tan, and you won&rsquo;t go far wrong.</p>
<p>With the exception of true dress shoes, like Oxfords, there aren&rsquo;t many shoes that won&rsquo;t pair well with raw jeans. That said,&nbsp;<a href="https://www.apetogentleman.com/how-to-wear-boots-with-jeans-men/" data-wpel-link="internal">boots always work well</a>&nbsp;as they fit in with raw denim&rsquo;s workwear roots &ndash; think moc-toe boots, chukka and&nbsp;<a href="https://www.apetogentleman.com/best-designer-hiking-boots-men/" data-wpel-link="internal">hiking boots</a>&nbsp;in either suede or leather.</p>
<div id="apeto-134f3a6c62176f358e8227e7a8007366" class="apeto-134f3a6c62176f358e8227e7a8007366 apeto-in-content-ads">
<div id="apeto-1674110262" class="apeto-in-content-ads"></div>
</div>
<div id="apeto-73ae5f3ba78997263c8fb6c9bc99cda8" class="apeto-73ae5f3ba78997263c8fb6c9bc99cda8 apeto-in-content-ad-after-intro">
<div id="apeto-948093548" class="apeto-in-content-ad-after-intro"></div>
</div>', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Perfect-Pairings-What-Shoes-To-Wear-With-Jeans', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'557e197f-14b5-4d61-8589-08e6b222e99d', N'The Best Luxury Smartwatches A Tech-Loving Collector Can Own', N'', N'~/Content/img/blog/Montblanc-Summit-2-1160x677.jpg', N'<p>Ah smartwatches, the final death of true watchmaking. The end is nigh! Well, not exactly. In fact, unlike quartz a few decades ago, the watch world has pretty much shrugged off smartwatches, which makes sense; they&rsquo;re two very different ball games.</p>
<p>On the one hand you have ancient crafts, Swiss artistry and maisons with archives that go back way before Steve Jobs was a twinkle in his daddy&rsquo;s eye. On the other: wearable mobiles with built-in redundancy, useful for the day-to-day but not exactly&nbsp;<a href="https://www.apetogentleman.com/best-investment-watches-2021/" data-wpel-link="internal">a long-term investment</a>.</p>
<p>However, that hasn&rsquo;t stopped some&nbsp;<a href="https://www.apetogentleman.com/best-luxury-watch-brands/" data-wpel-link="internal">watchmakers</a>&nbsp;embracing smart tech. While they&rsquo;re certainly still outliers, brands like TAG Heuer, Hublot and Montblanc have been pretty successful at integrating connectivity and health tracking into their signature cases.</p>
<p>So then, here are the best smartwatches that every watch lover can wear and be proud of.</p>
<h2>Apple Watch</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/apple-watch-series-6.jpg" /></p>
<p>The OG smartwatch. We shouldn&rsquo;t really need to tell you much about the almighty Apple Watch. It was the biggest wearable tech to hit the market when it first launched back in the halcyon days of 2015 and now that it&rsquo;s into its sixth generation the tech giant has added yet more bells and whistles onto what&rsquo;s already a one-man band of everyday life hacks.</p>
<p>Along with the usual host of wellness and fitness apps, the newest iteration can measure your blood oxygen level, includes an always-on retina display and an entire Fitness+ programme. It&rsquo;s also as sleek and painfully cool as ever.</p>
<p>It&rsquo;s not a luxury smartwatch in the same way as the others on this list, but if you&rsquo;re more tech-oriented, we wouldn&rsquo;t go for anything else.</p>
<h2>TAG Heuer Connected Super Mario Edition</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/tag-supermario.jpg" /></p>
<p>Most of the time TAG Heuer&rsquo;s uber-luxury take on the fitness wearable is smarter than a MENSA tuxedo, but its latest edition has taken the Swiss-made bones of the Connected, swallowed a moving mushroom whole and draped the whole thing in the signature red of Super Mario.</p>
<p>At its core, this is the same Connected that we&rsquo;ve seen for years, with all the fitness data any gym-obsessed fit freak could dream of. As well as the aesthetic tweaks however, this edition &lsquo;gamifies&rsquo; the whole experience, with the world&rsquo;s most famous plumber showing off a series of different animations according to how far through your fitness goals you get. Want to see him jump out of a pipe? Best hit 50%. Grab an invincibility star? That&rsquo;s your 75%. At 100%, the diminutive Italian will climb a goal pole and you&rsquo;re done for the day.</p>
<p>Whether it&rsquo;s too much of a novelty to warrant it&rsquo;s price tag is definitely a debate to be had, but as far as we&rsquo;re concerned 2,000 pieces is nowhere near enough. That&rsquo;s a smaller run than the NES Classic (and maybe worth as much in years to come).</p>
<h2>Hublot Big Bang E Premier League Edition</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/hublot-premier.jpg" /></p>
<p>If TAG Heuer hinted that&nbsp;<a href="https://www.apetogentleman.com/luxury-smartwatches/" data-wpel-link="internal">a luxury connected watch</a>&nbsp;could work, Hublot proved it with its football-oriented Big Bang E, which offers the over-the-top proportions of the base Big Bang case with plenty of smart tech.</p>
<p>The latest hams up the ties between Hublot and the beautiful game more than ever before with the Big Bang E Premier League limited edition. It&rsquo;s draped in the league&rsquo;s signature purple livery, but also includes a dedicated app to keep track of match times and live scores on the wrist. It even drills deeper, with team line-ups, VAR decisions and countdowns to the next fixture, giving you enough time to get to the nearest pub.</p>
<p>At &pound;4,300, it&rsquo;s one of the priciest smartwatches out there &ndash; but unlike the Super Mario TAG it at least takes itself seriously.</p>', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'The-Best-Luxury-Smartwatches-A-Tech-Loving-Collector-Can-Own', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'cd1509ab-1ab3-4572-a6fe-6b0e2707c88a', N'High-Fashion Scents: The Greatest Designer Fragrances Of All Time', N'', N'~/Content/img/blog/designer-fragrances-2-1160x677.jpg', N'Eau de Cologne – the style and type of concentration – was invented by Johann Maria Farina in 1709, while synthetic ingredients have been commonly used since the late 18th Century. And designer scents? “The men’s fashion market was born in the late 1950s, with the iconic launch of Monsieur de Givenchy,” says perfumer Azzi Glasser, founder of The Perfumer’s Story. “Brut by Fabergé (1968) brought in the famous fougère accord, which established the character for men’s fragrances and the start of many others,” says Glasser, who has created fragrances for Topman, Agent Provocateur and Bella Freud, and bespoke scents for many Hollywood actors.', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'High-Fashion-Scents-The-Greatest-Designer-Fragrances-Of-All-Time', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'b7d83c78-47e6-49cf-96ed-8eed5b055872', N'Blue-Collar Style: A Modern Man’s Guide To Denim Shirts', N'', N'~/Content/img/blog/best-denim-shirts-men-1160x677.jpg', N'<p>The denim shirt: one of menswear&rsquo;s true unsung heroes. This wardrobe workhorse lives in the shadow of so-called essentials like the&nbsp;<a href="https://www.apetogentleman.com/oxford-cloth-button-down-shirt/" data-wpel-link="internal">OCBD</a>&nbsp;and the&nbsp;<a href="https://www.apetogentleman.com/best-flannel-shirts-men/" data-wpel-link="internal">flannel shirt</a>, but it&rsquo;s every bit as versatile, easy to wear and functional. We&rsquo;d argue, even more so in some ways.</p>
<p>What we have here is a genuine workwear staple; a garment that&rsquo;s been there from the very beginning and remained steadfast throughout. From the workshops of 19th century America to the runway, the denim shirt has risen through the ranks, unfaltering along the way. Today, it&rsquo;s status as a must-have garment for the modern man is cemented and if you don&rsquo;t already count one among&nbsp;<a href="https://www.apetogentleman.com/types-of-shirt/" data-wpel-link="internal">your shirt selection</a>, well, what are you waiting for?</p>
<p>Here we take a look at some key considerations to make when shopping for your new denim shirt, along with some foolproof styling tips and our hand-picked list of the brands that do it best.</p>
<h2>Denim Shirt Buying Considerations</h2>
<p>There are a few things to think about in order to find the denim shirt that&rsquo;s best for you. From how it fits to where it&rsquo;s made, these are the key buying considerations every shopper should make before parting with their cash.</p>
<h3>Fit</h3>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/denimshirtbrandsproductslb3.jpg" /></p>
<p>Fit matters. In fact, it&rsquo;s probably the single most important thing about any given article of clothing. It doesn&rsquo;t matter how expensive or well made a garment is, if it doesn&rsquo;t fit, you&rsquo;ll still look badly dressed.</p>
<p>It can be a subjective thing, though. For example, you may be deliberately looking for a slouchy, oversized fit, which is fine. However, to play it safe and keep things classic, you should always be aiming for seams that sit neatly on the shoulders, a hem that falls a few inches south of your beltline and a body that is slim without being in any way tight or restrictive.</p>
<h3>Style</h3>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/denimshirtbrandsproductslb2.jpg" /></p>
<p>There are many different denim shirts out there, but most of them can be lumped into one of two distinct categories: western and classic button-down. Western shirts tend to feature accentuated, pointed yokes and dual chest pockets with snap openings. They sometimes (although less commonly) feature embellishments such as embroidery or fringes too.</p>
<p>The classic button-down style of denim shirt, on the other hand, is much like a simple OCBD, the key difference being that it&rsquo;s made of denim as opposed to Oxford cloth.</p>
<h3>Colour</h3>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/08/denimshirtbrandsproductslb.jpg" /></p>
<p>Denim shirts come in a fairly limited palette. Outside of black and various shades of grey and blue, there&rsquo;s really not a lot of choice. Which colour you eventually end up with will obviously be largely down to personal preference, but we&rsquo;d suggest sticking to a mid- to light-wash blue for a combination of classic looks and versatility. A darker, unwashed indigo can work well too &ndash; particularly for smart-casual looks.</p>
<div id="apeto-23ddafc0929b75a1a2424fc5d55035b9" class="apeto-23ddafc0929b75a1a2424fc5d55035b9 apeto-in-content-ads">
<div id="apeto-1766863416" class="apeto-in-content-ads"></div>
</div>
<div id="apeto-f38e80c2814adaf7dc945a7acfdf707a" class="apeto-f38e80c2814adaf7dc945a7acfdf707a apeto-in-content-ad-after-intro">
<div id="apeto-1031536189" class="apeto-in-content-ad-after-intro"></div>
</div>', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Blue-Collar-Style-A-Modern-Man’s-Guide-To-Denim-Shirts', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'aee5015a-6429-4f9a-8136-b194fb0dcefb', N'Summer Shirting: The Complete Short Sleeve Button Up Guide', N'', N'~/Content/img/blog/Short-sleeve-shirt-1160x677.jpg', N'For all its benefits, summer can be a tricky season to dress for. Not in terms of effort – after all, what could be easier than throwing on a pair of shorts and a top? – more that it can be difficult to get creative when limited to such a small selection of garments.Winter brings with it an abundance of sartorial avenues for exploration. We can layer, experiment with textures and use outerwear to our advantage. In summer the options are far fewer. But there is one handy tool you have at your disposal, provided you know how to use it to full effect.This summery take on the classic button-up is a style-conscious man’s best friend in the warmer months. It can lend a dressier edge to otherwise sloppy poolside ensembles, spice up uninspired outfits via appealing colours and eye-catching patterns, and is one of the few garments through which it’s acceptable to convey a little touch of humour and irony. All-over tropical island print? Don’t mind if we do.', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Summer-Shirting-The-Complete-Short-Sleeve-Button-Up-Guide', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'08bbfed9-2fe7-440e-95de-c29b1f34dd85', N'Fashion Forward: The Menswear Trends To Know For Fall/Winter 2021', N'', N'~/Content/img/blog/Menswear-trends-autumn-winter-2021-1160x677.jpg', N'<p>There&rsquo;s a common misconception that religiously adhering to trends equates to&nbsp;<a href="https://www.apetogentleman.com/how-to-dress-well-rules/" data-wpel-link="internal">good dressing</a>. In our humble opinion, this is not the case. Blindly following each and every seasonal trend is a recipe for poor style. Not to mention an easy way to bankrupt yourself.</p>
<p>The key to retaining your sartorial self-respect lies in the ability to successfully differentiate between the fleeting fads and the future classics. And in order to give you a nudge in the right direction, we&rsquo;ve created a carefully selected edit of the menswear movements worth incorporating into your wardrobe this year.</p>
<p>From the return of florals to the continued widening of silhouettes, these are the men&rsquo;s fashion trends to embrace in 2021.</p>
<h2>WFH Cosiness</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2021/09/119564730_1891739017632787_6994662396377016206_n-1160x769.jpg" /></p>
<p>It&rsquo;s impossible to say where we&rsquo;ll be 10 months from now but chances are there&rsquo;ll still be fluctuating coronavirus restrictions around the world. For that reason,&nbsp;<a href="https://www.apetogentleman.com/best-mens-loungewear-brands/" data-wpel-link="internal">luxe loungewear</a>&nbsp;and house shoes are likely to be trending hard.</p>
<p>Think socks &amp; &lsquo;stocks, hoodies, slouchy overshirts, drawstring pants and loose-fitting tees. The kind of clothing that borders on pyjama levels of comfort but that won&rsquo;t get you sacked if you have to attend an impromptu Zoom call with your boss.</p>
<h2>Fleece</h2>
<p><img src="https://www.apetogentleman.com/wp-content/uploads/2020/01/fleece.jpg" /></p>
<p>The revival of fleece is symptomatic of the broader trend in menswear towards outdoors-inspired garb. Thick-pile retro fleeces have been popular as standalone outerwear for several seasons now, while designers continue to work more and more of the fabric into their autumn/winter collections.</p>
<p>This trend is best served in small portions. Keep it to one fleece garment per outfit to avoid going full sheep and mix and match it with other textured fabrics to add another tactile dimension to your cold-weather looks.</p>', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Fashion-Forward-The-Menswear-Trends-To-Know-For-Fall-Winter-2021', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'5cc73fcd-ccaf-408f-861c-cc7b5ab9ab62', N'Win A Marloe Watch Company Watch', N'', N'~/Content/img/blog/Marloe-Watch-Company-1160x677.jpg', N'While today Marloe Watch Company is located up in Perth, Scotland, it was in Marlow that founder Oliver Goffe first discovered the downsides of many a quartz timepiece. Sure, they keep near-perfect time, but open up the back when you need to change the battery and you’ll find out that they often don’t exactly feel ‘prestige’, despite the sometimes prestige price tag.', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Win-A-Marloe-Watch-Company-Watch', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Article] ([articleId], [title], [description], [image], [content], [memberId], [meta], [status], [dateCreate]) VALUES (N'a9a407fe-cca9-4a0a-8dbe-f3f91845f24f', N'Low-Tech Kicks: Why The Humble Tennis Shoe Will Always Be In Fashion', N'', N'~/Content/img/blog/Adidas-Stan-Smith-1160x677.jpg', N'One hundred and twenty years ago this year, one Charles Taylor was born. Taylor, a one-time basketball player, had been a shoe salesman for most of his life. And yet his legacy was to establish a multi-billion dollar industry. During the 1930s, in one of the earliest instances of celebrity sponsorship, Charles, better known as ‘Chuck’, lent his name to a minimalistic high-top basketball boot that had been launched in 1917. And the result – the Chuck Taylor All-Star – was arguably the first classic, must-have sneaker.', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Low-Tech-Kicks-Why-The-Humble-Tennis-Shoe-Will-A-lways-Be-In-Fashion', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[Invoince] ([invoinceId], [deliveryStatus], [deliveryDate], [totalMoney], [memberId], [meta], [status], [dateCreate]) VALUES (N'd7ed2b52-d4c3-44f1-b0fd-6947ed06a7e5', 0, NULL, 48, N'6c49ebaf-d3be-4965-b025-a81a6a244298', NULL, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[Member] ([memberId], [phone], [password], [firstName], [lastName], [email], [birthday], [address], [avatar], [roleId], [meta], [status], [dateCreate]) VALUES (N'6c49ebaf-d3be-4965-b025-a81a6a244298', N'0338650639', N'5a62940e5f3f7dea0f3d803e475e4008', N'Hồng', N'Lê', N'hongle@gmail.com', CAST(N'2002-04-25' AS Date), N'779 QL50, Bình Chánh, TPHCM', N'~/Content/img/avatar/black-small-v.png', N'54ed1855-5103-4121-811c-3997ce4c2241', NULL, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Member] ([memberId], [phone], [password], [firstName], [lastName], [email], [birthday], [address], [avatar], [roleId], [meta], [status], [dateCreate]) VALUES (N'fe50d6de-ad78-416f-9521-ce1b40cb0e1c', N'0338650638', N'5a62940e5f3f7dea0f3d803e475e4008', N'Helen', N'Đoàn', N'helendoan@gmail.com', CAST(N'2000-12-08' AS Date), N'thành phố Quy Nhơn, tỉnh Bình Định', N'~/Content/img/avatar/me1.jpg', N'b69f924e-629c-4b58-b051-9634f4a12604', NULL, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Member] ([memberId], [phone], [password], [firstName], [lastName], [email], [birthday], [address], [avatar], [roleId], [meta], [status], [dateCreate]) VALUES (N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'admin', N'21232f297a57a5a743894a0e4a801fc3', N'User', N'Admin', N'admin@gmail.com', CAST(N'2002-03-18' AS Date), N'300A Nguyễn Tất Thành, Phường 13, Quận 4, Tp.HCM', N'~/Content/img/avatar/244378234_2621616374649091_7842324519169438481_n.jpg', N'b14d0769-e485-4455-93bc-c39007910115', NULL, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[Menu] ([menuId], [name], [meta], [status], [order], [dateCreate]) VALUES (N'f224882c-fdd3-458b-8cd2-5f353ccf6f0a', N'Product', N'san-pham', 1, 2, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Menu] ([menuId], [name], [meta], [status], [order], [dateCreate]) VALUES (N'5be8cb03-6153-49c7-af2a-6b79072c9b7e', N'Home', N'trang-chu', 1, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Menu] ([menuId], [name], [meta], [status], [order], [dateCreate]) VALUES (N'28df2823-e8b0-4f9a-82f9-a70ed26bbcad', N'Article', N'bai-viet', 1, 3, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Menu] ([menuId], [name], [meta], [status], [order], [dateCreate]) VALUES (N'463151cc-5805-4335-a93c-facdb2f8283d', N'Contact', N'lien-he', 1, 4, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'cb9d7f8e-ca79-4dbf-8b5c-05732ef286da', N'Giày Crazy In Creation Ver.1 Da Suede', N'~/Content/img/product/SB002_01.jpg', 720000, 2, N'Form: Shoes, Chất liệu: Canvas, Thiết kế: Phối màu', N'NULL', 0, NULL, N'854866c9-7d04-4938-ae03-e9fb56730606', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Giay-Crazy-In-Creation-Ver-1-Da-Suedel-SB002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'6f1a146b-995d-4e8c-a609-22839cd38990', N'Áo Thun Tay Ngắn Phối Dây Rút Form Loose', N'~/Content/img/product/SI001_01.jpg', 429000, 2, N'Form: Loose, Thiết kế: Họa tiết thêu, Kiểu tay: Tay ngắn', N'NULL', 0, NULL, N'502ad263-3340-4613-b7b1-92b7428395fc', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Thun-Tay-Ngan-Phoi-Day-Rut-Form-Loose-SI001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'f5d113ac-606a-42f4-a9d1-36b970dc5b59', N'Quần Jean Trơn Form Slim Crop', N'~/Content/img/product/JE001_01.jpg', 480000, 2, N'Form: Slim Crop, Chất liệu: Jeans, Thiết kế: Trơn', N'<p>Quần Jean Nam Trơn Form Slim Crop là kiểu quần jean luôn nhận được sự săn đón của tín đồ thời trang. Với thiết kế kiểu jean đen, trơn có thể phối với mọi loại trang phục để đa dạng phong cách cho người mặc. Chất liệu hiện đại, dày dặn với ưu điểm thoáng khí, độ bền cao. Kết hợp form quần slim phổ biến, là dáng quần ôm vào cơ thể nhưng không bó sát vẫn vừa vặn, thoải mái cho người mặc.</p><p>Quần jean đen chắc hẳn là một item thần thánh mang phong cách casual được thiết kế dành cho các chàng trai theo đuổi sự hiện đại, trẻ trung trong phong cách thời trang của mình.</p>', 0, NULL, N'ba6da55e-a160-4afe-9e65-75a27c65be61', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Jean-Tron-Form-Slim-Crop-JE001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'bc0dcd4f-144e-4dec-a7a2-36e70a04ea25', N'Áo Blazer Phối Cổ Form Fitted', N'~/Content/img/product/BL001_01.jpg', 1249000, 2, N'Form: Fitted, Chất liệu: Polyester, Thiết kế: Phối màu, Kiểu tay: Tay dài', N'<p>Áo vest không những đã trở thành một bộ trang phục quen thuộc mà còn là sự lựa chọn hàng đầu để các quý ông thể hiện sự mạnh mẽ và quyền lực của mình. Thường khi nhắc đến vest, người ta sẽ cho rằng nó được ứng dụng vào môi trường làm việc công sở hoặc những buổi tiệc sang trọng là chủ yếu. Tuy nhiên, trong cuộc sống hiện đại ngày nay, với sự phát triển về kiểu dáng và mẫu vest lại được ứng dụng một cách phổ biến hơn cho cả những buổi hẹn hò, dạo phố.</p><p>Áo Blazer Phối Cổ Form Fitted là sản phẩm chân ái dành cho những bạn nam cá tính, yêu thích những gì độc - đẹp - lạ. Sản phẩm này mang đến vẻ đẹp của sự độc đáo, phá cách và năng động cho riêng mình với chi tiết phối viền cổ và thiết kế tag nhỏ được đính ở phần tay trái áo. Mặc dù có thiết kế lạ mắt nhưng mẫu áo này vẫn đáp ứng được tiêu chí dễ phối đồ và tính ứng dụng cao của khách hàng nhờ gam màu trung tính. Cuối cùng, chàng có thể yên tâm hoạt động mà không sợ mất đi vẻ ngoài chỉn chu, gọn gàng của mình nhờ đặc trưng chống nhăn của chất vải polyester.</p>', 0, NULL, N'fe84cf0c-4a6b-457d-9e6b-ce9dcb308669', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Blazer-Phoi-Co-Form-Fitted-BL001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'94e88e55-f5dc-42c6-af8f-4192ab216b52', N'Áo Sơ Mi Tay Ngắn Sợi Coffee Trơn Form Fitted', N'~/Content/img/product/TS001_01.jpg', 390000, 2, N'Form: Fitted, Chất liệu: Coffee, Thiết kế: Trơn, Kiểu tay: Tay ngắn', N'<p>Áo sơ mi tay ngắn sợi coffee form fitted toát lên phong cách đầy tự tin, năng động. Áo được thiết kế với kiểu dáng tối giản, trẻ trung phù hợp khi đi học, đi chơi hay đi làm. Cùng với form áo tay ngắn, họa tiết trơn giúp dễ dàng phối với mọi outfit khác trong tủ đồ. Chiếc áo sơ mi sử dụng chất liệu từ coffee với ưu điểm nổi bật là kiểm soát hấp thụ mùi tốt kết hợp vải tái chế bền, nhẹ chính là điểm vượt trội trong chất liệu chiếc áo này.</p><p>Áo sơ mi tay ngắn từ lâu đã trở thành là một item quen thuộc trong thời trang. Từ thiết kế đơn giản, thanh lịch đến những thiết kế trẻ trung, năng động đã giúp áo sơ mi tay ngắn dần trở thành sự lựa chọn phổ biến trong phong cách thời trang thường ngày.</p>', 0, NULL, N'c6554646-699b-4755-9c88-4df944f21ada', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-So-Mi-Tay-Ngan-Soi-Coffee-Tron-Form-Fitted-TS001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'53ea81e7-4ffb-43ba-b075-42184d9645fc', N'Băng Đô Vải Polyester Form Fitted', N'~/Content/img/product/AJ004_01.jpg', 65000, 2, N'Form: Fitted, Chất liệu: Polyester', N'NULL', 0, NULL, N'7b09b9f1-46cb-48ea-8e1e-8606adc2b55d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Bang-Do-Vai-Polyester-Form-Fitted-AJ004', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'a1e083fa-f72a-4694-ab01-446b29d88f24', N'Quần Jogger Kaki Form Jogger', N'~/Content/img/product/JG002_01.jpg', 580000, 2, N'Form: Jogger, Chất liệu: Cotton, Thiết kế: Trơn', N'<p>Quần jogger ngày càng trở nên phổ biến, chúng thường được mọi người diện mọi nơi từ đi làm, dạo phố đến đi du lịch. Và hiện này, quần jogger đã trở thành một xu hướng thời trang được mọi người ưa chuộng.</p><p>Quần Jogger Kaki Form Jogger được may từ chất liệu tự nhiên mang những ưu điểm như thoáng mát, thấm hút mô hồi và chống mùi tốt. Form quần jogger mang phong cách năng động, cá tính với phần cuối của ống được may thêm thun, chun túm tạo gọn gàng và thuận tiện khi vận động. Quần được thiết kế nhiều túi mang đến sự tiện dụng cho người mặc, ngoài ra phần ống còn được may ghép mảnh đầy phá cách.</p>', 0, NULL, N'5c6e8355-cc4b-4881-9f64-4df5f99e52ee', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Jogger-Kaki-Form-Jogger-JG002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'53633fdd-9e31-4030-99f9-4aa738a58f8a', N'Quần Vải Form Slim Crop', N'~/Content/img/product/PA002_01.jpg', 480000, 2, N'Form: Slim Crop, Chất liệu: Tổng hợp, Thiết kế: Trơn', N'<p>Hiện nay quần vải nam được thay đổi rất nhiều từ màu sắc cho tới kiểu dáng. Ngày càng bắt mắt khách hàng hơn. Cũng như sự gọn gàng và thoải mái mà quần vải đem lại. Chính vì thế mà quần vải nam được rất nhiều các bạn nam yêu thích. Từ những chiếc quần cứng nhắc chỉ phù hợp với công sở. Mà giờ đây có thể kết hợp với nhiều trang phục vô cùng bắt mắt. Quần vải nam kết hợp áo sơ mi tạo sự chỉnh chu. Quần vải nam kết hợp áo phông cho chàng trai nét năng động nhưng vẫn chững chạc.</p><p>Quần Dài Slim Crop là trang phục quen thuộc, không thể thiếu đối với bất kỳ chàng trai nào. Quần được may với chất liệu cotton nên sờ vào mềm mịn và thoáng mát. Quần được thiết kế với form quần slim crop suông, thẳng có ống ngắn trên mắt cá chân thích hợp để các chàng mix với sneaker hoặc giày cao cổ. Là một item hiện đại và thanh lịch phù hợp để đi học, đi làm,...</p>', 0, NULL, N'48158d2d-7a04-4617-b4d8-57c7d8f355f0', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Vai-Form-Slim-Crop-PA002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'89ec5859-e6f0-49b8-9728-5107e319ebe3', N'Quần Vải Lưng Thun Form Slim Crop', N'~/Content/img/product/PA01_01.jpg', 529000, 2, N'Form: Slim Crop, Chất liệu: Polyester, Thiết kế: Trơn', N'<p>Hiện nay quần vải nam được thay đổi rất nhiều từ màu sắc cho tới kiểu dáng. Ngày càng bắt mắt khách hàng hơn. Cũng như sự gọn gàng và thoải mái mà quần vải đem lại. Chính vì thế mà quần vải nam được rất nhiều các bạn nam yêu thích. Từ những chiếc quần cứng nhắc chỉ phù hợp với công sở. Mà giờ đây có thể kết hợp với nhiều trang phục vô cùng bắt mắt. Quần vải nam kết hợp áo sơ mi tạo sự chỉnh chu. Quần vải nam kết hợp áo phông cho chàng trai nét năng động nhưng vẫn chững chạc.</p><p>Quần Vải Nam Lưng Thun Form Slim Crop là một item được thiết đơn giản, màu sắc trung tính dễ phối mang phong thanh lịch, hiện đại. Form quần vừa vặn với cơ thể, ống quần lửa mang đến sự trẻ trung, năng động và có thể dễ dàng mix&match cùng nhiều loại giày khác nhau như giày sneaker, giày cao cổ,... Chất liệu bền, có khả năng giữ form ổn định và chống nhăn tốt. Đặc biệt, lưng quần phía sau được mai lưng thun có sự co giãn tốt tạo được sự thoải mái cho người mặc.</p>', 0, NULL, N'48158d2d-7a04-4617-b4d8-57c7d8f355f0', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Vai-Lung-Thun-Form-Slim-Crop-PA001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'bc5ff0ee-773c-44f7-a8e2-66cbd728a8e5', N'Áo Polo Tay Ngắn Họa Tiết In Form Regular', N'~/Content/img/product/PS001_01.jpg', 449000, 2, N'Form: Regular, Chất liệu: 100% cotton, Thiết kế: Họa tiết in, Kiểu tay: Tay ngắn', N'<p>Áo Polo Tay Ngắn Họa Tiết In Form Regular được thiết kế trẻ trung, năng động, phù hợp với nhiều đối tượng ở nhiều lứa tuổi khác nhau. Với chất liệu từ cotton giúp vải áo được mềm, mịn giúp người mặc luôn cảm thấy thoải mái và dễ chịu khi diện chúng. Form áo cổ điển, hơi ôm vào người nhưng không quá sát để tạo sự thoáng mát, thấm hút mồ hôi khi vận động. Điểm nhấn với họa tiết in chữ giúp chiếc áo trẻ trung và hiện đại hơn.</p><p>Ngày nay, áo polo nam là loại trang phục không thể không có trong tủ đồ của nam giới. Polo là item linh hoạt có thể mặc được ở mọi nơi, mọi thời tiết. Chính vì vậy, áo polo trên hội tụ đủ những yếu tố mà các chàng trai cần: lịch sự, chỉn chu nhưng lại không quá formal.</p>', 0, NULL, N'2aa717ac-f048-4b93-9507-188d0f84138d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Polo -Tay-Ngan-Hoa-Tiet-In-Form-Regular-PS001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'674fb355-2d56-49e7-8ef1-6cb5c5e589dd', N'Balo Da Trơn Form Basic', N'~/Content/img/product/SB004_01.jpg', 749000, 2, N'Thiết kế: Trơn', N'NULL', 0, NULL, N'854866c9-7d04-4938-ae03-e9fb56730606', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Balo-Da-Tron-Form-BasicSB004', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'ea98ab09-fa55-4609-b77f-8bd4de7dd107', N'Áo Khoác Bomber Form Regular', N'~/Content/img/product/OW002_01.jpg', 1150000, 2, N'Form: Regular, Kiểu tay: Tay dài', N'<p>Mùa hè là mùa có thời tiết nóng nhất quanh năm. Ánh nắng gắt khiến bất kì ai cũng khó chịu, có thể gây nguy hại đến mắt và làn da nếu không che nắng cẩn thận. Áo khoác là loại trang phục nhỏ, gọn, đa-zi-năng: vừa có thể giữ ấm cơ thể vào mùa lạnh, vừa có khả năng cách nhiệt, chống nắng vào mùa nóng. Không chỉ vậy, áo khoác còn mang đến cho các chàng trai những phong cách thời thượng, khỏe khoắn, năng động.</p><p>Áo Khoác Cotton Form Regular có form áo regular được biết đến với khả năng dễ phối đồ và mang đến sự phóng khoáng, thoải mái cho tổng thể outfit. Thân áo không được may phẳng như áo khoác thông thường tạo cho sản phẩm sự độc đáo và lạ mắt hơn. Hơn nữa, được dệt từ chất liệu 100% cotton với độ thoáng mát và thấm hút mồ hôi vượt bậc sẽ giúp sản phẩm phù hợp để diện bất kể mùa nào trong năm.</p>', 0, NULL, N'e54ae3d9-3cbe-47f7-bbce-8a96b4802e52', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Khoac-Bomber-Form-Regular-OW002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'e3ee3773-44e7-4a8c-9fde-900d0a2c0843', N'Áo Hoodie Tay Dài In Hình Form Regular', N'~/Content/img/product/HD001_01.jpg', 650000, 2, N'Form: Regular, Chất liệu: Cotton, Thiết kế: Họa tiết in, Kiểu tay: Tay dài', N'<p>Đã là một tín đồ thời trang thì chắc hẳn bạn sẽ không thể nào bỏ lỡ những chiếc áo hoodie, một chiếc áo đang ngày càng trở nên phổ biến và trở thành một xu hướng thời trang không ngừng hot. Đây là một item thể thao mang tính ứng dụng cao, nó có thể dễ dàng mix cùng những trang phục quen thuộc thường ngày như chân quần jean, quần short,... Ngoài ra, áo hoodie còn là một món đồ không ngừng được săn đón vào mùa đông bởi khả năng giữ ấm cực tốt.</p><p>Áo Hoodies Tay Dài In Cotton Form Regular là form áo phổ biến nhất hiện nay với thiết kế vừa vặn với cơ thể tạo cảm giác thoải mái cho người mặc. Thiết kế không khóa và có túi lớn ở phần trước bụng có công dụng giữ ấm tay cực tốt. Ngoài ra, còn có thiết kế họa tiết thêu nhỏ xinh ở phần ngực áo tạo thêm điểm nhấn cho chiếc áo. Đây là một chiếc áo được thiết kế cho các chàng trai trẻ trung, năng động và đầy cá tính.</p>', 0, NULL, N'dcf1c535-f05f-45cd-9489-06576a4933d3', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Hoodie-Tay-Dai-In-Hinh-Form-Regular-HD001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'e59179ce-dd99-4213-ac94-a0b11ce23159', N'Quần Jogger Nylon Phối Chỉ Form Jogger', N'~/Content/img/product/JG001_01.jpg', 580000, 2, N'Form: Jogger, Chất liệu: Nylon, Thiết kế: Trơn', N'<p>Quần jogger ngày càng trở nên phổ biến, chúng thường được mọi người diện mọi nơi từ đi làm, dạo phố đến đi du lịch. Và hiện này, quần jogger đã trở thành một xu hướng thời trang được mọi người ưa chuộng.</p><p>Quần Jogger Nylon Phối Chỉ Form Jogger được may từ chất liệu có khả năng chống nước, cản gió và giữ ấm cơ thể tốt. Có form dáng jogger đầy cá tính, hiện đại, thêm vào đó ở phần cuối của ống quần còn có thêm chun túm vừa tạo được cảm giác gọn gàng và tạo sự thuận tiện và thoải mái khi mặc. Điểm nổi bật nhất của chiếc quần chính là chi tiết đường viền được phối chỉ đầy mới lạ, mang một vẻ đẹp phá cách.</p>', 0, NULL, N'5c6e8355-cc4b-4881-9f64-4df5f99e52ee', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Jogger-Nylon-Phoi-Chi-Form-Jogger-JG001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'653d5fb2-7600-48b0-95f2-a5cf687e5d31', N'Túi Đeo Chéo Phối Màu Thời Trang', N'~/Content/img/product/SB003_01.jpg', 349000, 2, N'Thiết kế: Phối màu', N'NULL', 0, NULL, N'854866c9-7d04-4938-ae03-e9fb56730606', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Tui-Deo-Cheo-Phoi-Mau-Thoi-Trang-SB003', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'4a2a10be-ae19-40e1-95ec-a62d35ae1103', N'Áo Tanktop Thể Thao Trơn Form Regular', N'~/Content/img/product/TT002_01.jpg', 169000, 2, N'Form: Regular, Chất liệu: Polyester, Thiết kế: Trơn, Kiểu tay: Sát nách', N'<p>Dù là sáng hay tối, ngày trong tuần hay cuối tuần. Dù là trong phòng tập với đầy đủ thiết bị hay ngoài không gian mở với không khí thoáng đãng. Dòng sản phẩm thiết yếu activewear từ Routine sẽ là người bạn đồng hành để không chỉ giúp bạn vận động thoải mái nhất mà sẽ có thêm tự tin và nhiều cảm hứng trong việc tập luyện và hoạt động thể thao thường ngày nhờ vào những thiết kế thời trang và rất dễ mặc.</p><p>Áo Tanktop Thể Thao Nam Sát Nách Trơn Form Regular mang form áo suông thẳng, ôm vừa vặn với cơ thể giúp tôn lên vóc dáng săn chắc của các chàng. Được may từ chất liệu mỏng nhẹ, thoáng mát rất phù hợp với các hoạt động thể thao. Thêm thiết kế sọc ở vai áo mang một điểm nhất nổi bật. Là một item dành cho các chàng yêu thích sự năng động, trẻ trung.</p>', 0, NULL, N'47eb74cf-5eb1-4f67-88dd-1b1e3161372f', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Tanktop-The-Thao-Tron-Form-Regular-TT002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'e8a3a0dc-05a2-4dcb-b1d8-ae7207ff0e2e', N'Quần Short Ống Rộng Nhãn Trang Trí Form Relax', N'~/Content/img/product/SO02_01.jpg', 350000, 2, N'Form: Relax, Chất liệu: Cotton, Thiết kế: Trơn', N'<p>Quần short nam đã trở thành xu hướng thời trang nối bật hiện nay. Với những ưu điểm vượt trội như gọn gàng và tiện dụng, đặc biệt là khi thời tiết trở nên nóng bức hơn thì quần short nam sẽ là một item không thể thiếu trong tủ đồ của cánh mày râu. Ngoài ra, bạn còn có thể dễ dàng tìm được cho mình một item phù hợp nhất bởi sự đa dạng kiểu dáng và màu sắc của nó.</p><p>Quần Short Nam Ống Rộng Nhãn Trang Trí Form Relax được làm từ sợi bông tự nhiên, sờ mịn tay và mang ưu điểm vượt trội là thoáng mát, thấm hút mồ hôi tốt. Là chiếc quần thể thao ống rộng, kèm theo lưng quần có dây rút vô cùng tiện dụng. Ngoài ra, quần còn được thêm chi tiết nhãn dán ở phần ống tạo điểm nhấn cho chiếc quần và mang đến sự trẻ trung và năng động cho người mặc.</p>', 0, NULL, N'400f03a0-ff63-42bf-8e8e-87176386c5a7', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Short-Ong-Rong-Nhan-Trang-Tri-Form-Relax-SO001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'b360c906-a0c5-41f5-9b9e-bbc87ea8768c', N'Dây Nịt Lưng Trơn Freesize', N'~/Content/img/product/AJ003_01.jpg', 450000, 2, N'Chất liệu: 100% leather, Thiết kế: Trơn', N'<p>Dây Nịt là item không thể thiếu trong tủ quần áo mà còn là một bảo bối giúp hoàn thiện outfit thời trang của cánh mày râu. Được thiết kế đơn giản theo kiểu dáng khóa kim thanh lịch, nhẹ nhàng. Kết hợp với chất liệu da thuộc mềm và dẻo tạo cảm giác dễ chịu khi sử dụng. Với thiết kế này bạn có thể tự tin phối cùng nhiều loại trang phục khác nhau từ quần âu thanh lịch, đến quần jean năng động.</p>', 0, NULL, N'7b09b9f1-46cb-48ea-8e1e-8606adc2b55d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Day-Nit-Lung-Tron-Freesize-AJ003', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'41889f70-25e4-4ad5-a5ee-c48447d543d8', N'Quần Short Knit Trơn Form Slim', N'~/Content/img/product/SO01_01.jpg', 480000, 2, N'Form: Slim, Chất liệu: Polyester, Thiết kế: Trơn', N'<p>Quần short nam đã trở thành xu hướng thời trang nổi bật hiện nay. Với những ưu điểm vượt trội như gọn gàng và tiện dụng, đặc biệt là khi thời tiết trở nên nóng bức hơn thì quần short nam sẽ là một item không thể thiếu trong tủ đồ của cánh mày râu. Ngoài ra, bạn còn có thể dễ dàng tìm được cho mình một item phù hợp nhất bởi sự đa dạng kiểu dáng và màu sắc của nó.</p><p>Quần Short Nam Knit Trơn Form Slim là một thiết kế điển hình của quần short nam. Với màu sắc nhẹ nhàng, thanh lịch và form quần phổ biến có phần đùi và mông thoải mái, không bó sát dễ dàng cử động. Được thiết kế đơn giản nên có thể dễ dàng phối hợp cùng cách trang phục khác tạo ra nhiều phong cách khác nhau phù hợp với gout ăn mặc của từng người.</p>', 0, NULL, N'400f03a0-ff63-42bf-8e8e-87176386c5a7', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Short-Knit-Tron-Form-Slim-SO001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'a64aaa8c-9ce0-442f-a0f3-ce6238ddc81e', N'Áo Khoác Nhung Cotton Form Regular', N'~/Content/img/product/OW001_01.jpg', 1150000, 2, N'Form: Regular, Chất liệu: Cotton, Kiểu tay: Tay dài', N'<p>Mùa hè là mùa có thời tiết nóng nhất quanh năm. Ánh nắng gắt khiến bất kì ai cũng khó chịu, có thể gây nguy hại đến mắt và làn da nếu không che nắng cẩn thận. Áo khoác là loại trang phục nhỏ, gọn, đa-zi-năng: vừa có thể giữ ấm cơ thể vào mùa lạnh, vừa có khả năng cách nhiệt, chống nắng vào mùa nóng. Không chỉ vậy, áo khoác còn mang đến cho các chàng trai những phong cách thời thượng, khỏe khoắn, năng động.</p><p>Áo Khoác Cotton được thiết kế theo hình dạng của một chiếc áo sơ mi với form áo rộng, suông thẳng đứng từ trên xuống, có chiều dài ngang mông. Áo được làm từ chất liệu cotton thoáng mát, mịn tay, thấm hút mồ hôi tốt và mặt vải hình gân trẻ trung, cá tính. Bên trong áo còn có thiết kế túi vừa tiện dụng, vừa hiện đại. Phần góc dưới của thân áo có kèm họa tiết thêu nhẹ nhàng, sinh động. Điểm đặc biệt nhất chính là phần lay áo được thiết kế vô cùng độc đáo, bản to và có khuy cài bên hông áo giúp người mặc có thể dễ dàng điều chỉnh độ bó của lai áo để biến tấu phù hợp cũng phong cách của mình.</p<', 0, NULL, N'e54ae3d9-3cbe-47f7-bbce-8a96b4802e52', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Khoac-Nhung-Cotton-Form-Regular-OW001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'c50d49d3-f961-4bc7-b862-d9beb46a27df', N'Quần Jean Trơn Form Slim', N'~/Content/img/product/JE002_01.jpg', 599000, 2, N'Form: Slim, Chất liệu: Jeans, Thiết kế: Trơn', N'NULL', 0, NULL, N'ba6da55e-a160-4afe-9e65-75a27c65be61', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Quan-Jean-Tron-Form-Slim-JE002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'67cccd69-f3b1-4156-b205-e10772f6481d', N'Áo Hoodie Thể Thao Tay Ngắn Có Nón Form Regular', N'~/Content/img/product/HD002_01.jpg', 890000, 2, N'Form: Regular, Kiểu tay: Tay ngắn', N'<p>Dù là sáng hay tối, ngày trong tuần hay cuối tuần. Dù là trong phòng tập với đầy đủ thiết bị hay ngoài không gian mở với không khí thoáng đãng. Dòng sản phẩm thiết yếu activewear từ Routine sẽ là người bạn đồng hành để không chỉ giúp bạn vận động thoải mái nhất mà sẽ có thêm tự tin và nhiều cảm hứng trong việc tập luyện và hoạt động thể thao thường ngày nhờ vào những thiết kế thời trang và rất dễ mặc.</p><p>Áo Hoodie Thể Thao Tay Ngắn Có Nón Form Regular với phong cách thiết kế phá cách, mới lạ với form rộng, tay ngắn, có nón giúp cho người mặc tự tin phối nhiều style trang phục khác nhau. Chất vải pha Nylon Spandex kết hợp với kiểu dáng cổ điển, suông thẳng, vừa vặn vào phần cơ thể mang đến sự nhẹ nhàng, thoáng mát và dễ chịu cho người mặc. Áo hoodie form regular không chỉ đem đến sự thoải mái khi hoạt động mà còn giữ ấm trong thời tiết tập luyện lạnh.</p>', 0, NULL, N'dcf1c535-f05f-45cd-9489-06576a4933d3', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Hoodie-The-Thao-Tay-Ngan-Co-Non-Form-Regular-HD002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'b72e7153-2099-457e-a3c1-e2bfa603dacf', N'Áo Sơ Mi Tay Dài Kẻ Sọc Form Regular', N'~/Content/img/product/TS002_01.jpg', 550000, 2, N'Form: Regular, Chất liệu: Cotton, Thiết kế: Kẻ sọc, Kiểu tay: Tay dài', N'<p>Ngày nay, chúng ta không chỉ bắt gặp những chiếc áo sơ mi tay dài trong môi trường công sở như trước đây mà còn có thể thường xuyên nhìn thấy những bộ outfit có sự kết hợp với áo sơ mi trong đời sống thường ngày, các buổi tiệc sang trọng,… Sở dĩ, sơ mi tay dài phổ biến trong thời trang bởi vì khả năng dễ phối đồ, tính linh hoạt phù hợp trong nhiều hoàn cảnh và cuối cùng chính là làm nổi bật được vẻ thanh lịch, chỉn chu và sang trọng cho người mặc.</p><p>Áo Sơ Mi Tay Dài Nam Có Túi Cotton Form Regular được làm từ chất liệu cotton giúp áo thoáng mát kết hợp công nghệ dệt đặc biệt tạo nên những đường sọc rất ấn tượng. Phần form áo suông thắng đứng, độ rộng vừa phải mang lại sự thoải mái khi mặc, phần họa tiết tag áo may kèm theo với dòng chữ nhỏ xinh thêm phần nhấn nhá cho chiếc áo.</p>', 0, NULL, N'c6554646-699b-4755-9c88-4df944f21ada', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-So-Mi-Tay-Dai-Ke-Soc-Form-Regular-TS002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'7f179d89-9fdb-4500-b62b-e9a5eaa4b5ef', N'Áo Thun Tay Ngắn Họa Tiết Thêu Form Regular', N'~/Content/img/product/SI002_01.jpg', 349000, 2, N'Form: Regular, Chất liệu: 100% cotton, Thiết kế: Họa tiết thêu, Kiểu tay: Tay ngắn', N'<p>Áo Thun Tay Ngắn Họa Tiết Thêu Form Regular mang form dáng cổ điển của một chiếc áo thun, dáng suông thẳng đứng từ trên xuống nên có độ rộng cho người mặc thoải mái cử động. Được làm từ chất vải cotton tự nhiên mang đặc tính mền mịn, thoáng mát và thấm hút mồ hôi tốt. Điểm nhấn ấn tượng nhất của chiếc áo chính là phần nhãn được may tỉ mỉ trên túi áo và lai áo mang đến sự trẻ trung và cá tính hơn cho một chiếc áo thun basic.</p><p>Áo thun tay ngắn là một trong những item cơ bản không còn xa lạ thời trang thường ngày. Là lựa chọn ưu tiên hàng đầu của nam giới bởi sự đơn giản nhưng linh hoạt và ứng dụng cao. Áo thun tay ngắn dễ dàng phối với nhiều kiểu quần khác nhau để tạo nên nhiều outfit phù hợp phong cách mỗi người.</p>', 0, NULL, N'502ad263-3340-4613-b7b1-92b7428395fc', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Thun-Tay-Ngan-Hoa-Tiet-Theu-Form-Regular-SI002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'a2bdf0e0-5139-4ede-ab13-eea3041f85c2', N'Áo Tanktop Thun Kẻ Sọc Gân Trơn Form Slim', N'~/Content/img/product/TT001_01.jpg', 229000, 2, N'Form: Slim, Chất liệu: 95% cotton 5% spandex, Thiết kế: Trơn, Kiểu tay: Sát nách', N'<p>Áo Thun Tanktop Nam Sát Nách Kẻ Sọc Gân Trơn Form Slim được thiết kế là kiểu áo năng động, thoáng mát rất thích hợp cho mùa hè này:</p><p>Chất vải bền đẹp thêm khả năng thấm hút mồ hôi và co giãn tốt cho người mặc cảm giác thoải mái khi sử dụng</p><p>Form slim ôm vào cơ thể giúp người mặc trông gọn gàng và tôn dáng</p><p>Dáng áo sát nách thoáng mát và mang sự nam tính, cuốn hút</p><p>Thiết kế mặt vải kẻ sọc gân cho hiện đại tạo điểm nhấn ấn tượng cho chiếc áo</p><p>Là kiểu áo rất thích hợp cho các chuyến du lịch biển, đi chơi cùng bạn bè.</p>', 0, NULL, N'47eb74cf-5eb1-4f67-88dd-1b1e3161372f', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Tanktop-Thun-Ke-Soc-Gan-Tron-Form-Slim-TT001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'2fefb818-e3f5-4418-a872-f16b2ce22313', N'Áo Nỉ Sweater Tay Dài Thêu Logo Form Regular', N'~/Content/img/product/SS001_01.jpg', 450000, 2, N'Form: Regular, Thiết kế: Trơn, Kiểu tay: Tay dài', N'<p>Vải nỉ đã được phổ biến trên thị trường từ những năm 1990. Qua quá trình phát triển và cải tiến, vải nỉ đã chiếm lĩnh một phần không nhỏ trong thị trường vải vóc. Và trong lĩnh vực thời trang, nỉ thường được sử dụng để thiết kế nên các loại quần áo như áo hoodie, áo sweater, đồ trẻ em, đồ ngủ,...</p><p>Áo Sweater Thêu Logo Form Regular với form áo regular vừa vặn với cơ thể sẽ che được một số điểm ở phần thân trên mà chàng chưa thật sự tự tin nhưng vẫn đảm bảo vẻ ngoài của bạn nam trông vẫn chỉn chu và gọn gàng. Áo có 3 màu sắc trung tính vừa đem lại cho chàng nhiều sự lựa chọn vừa đảm bảo cho dù phối với mẫu quần nào hay phụ kiện nào vẫn sẽ giữ được vẻ ngoài nam tính, lịch lãm của bạn nam. Phần logo thêu nho nhỏ trên ngực trái làm điểm nhấn giúp sản phẩm không bị đại trà và nhàm chán.</p>', 0, NULL, N'c642afcd-60c9-489a-83df-34a4a6b90c9e', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Ni-SweaterTay-Dài-Theu-Logo-Form-Regular-SS001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'f386ad11-a67d-41bd-b5e2-f18f9d705363', N'Giày Sneaker Canvas Trơn Basic Poin Label', N'~/Content/img/product/SB001_01.jpg', 580000, 2, N'Form: Shoes, Chất liệu: Canvas, Thiết kế: Trơn', N'NULL', 0, NULL, N'854866c9-7d04-4938-ae03-e9fb56730606', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Giay-Sneaker-Canvas-Tron-Basic-Poin-Label-SB001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'f64e81a1-8ca4-4e60-bab6-f6f54ec2f3c6', N'Nón Bucket Phối Chỉ Họa Tiết Basic', N'~/Content/img/product/AJ001_01.jpg', 179000, 2, N'Thiết kế: Họa tiết thêu', N'NULL', 0, NULL, N'7b09b9f1-46cb-48ea-8e1e-8606adc2b55d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Non-Bucket-Phoi-Chi-Hoa-Tiet-Basic-AJ001', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'c4b3c1b6-e3bd-4fa5-b2b6-fb6e69807c40', N'Nón Lưỡi Trai Có Hình Thêu Freesize', N'~/Content/img/product/AJ002_01.jpg', 190000, 2, N'Chất liệu: Kaki, Thiết kế: Họa tiết thêu', N'NULL', 0, NULL, N'7b09b9f1-46cb-48ea-8e1e-8606adc2b55d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Non-Luoi-Trai-Co-Hinh-Theu-Freesize-AJ002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Product] ([productId], [productName], [image], [price], [discount], [characteristic], [description], [quanlity], [brand], [categoryId], [memberId], [meta], [status], [dateCreate]) VALUES (N'7e664b87-ba89-460a-b2d6-fd6eb43f451c', N'Áo Polo Tay Bo Phối Màu Form Fitted', N'~/Content/img/product/PS002_01.jpg', 519000, 2, N'Form: Fitted, Thiết kế: Phối màu, Kiểu tay: Tay ngắn', N'<p>Áo Polo Tay Bo Phối Màu Form Fitted được may từ chất vải Cotton thoáng mát, thấm hút mồ hôi tốt, chống mùi phù hợp mội hoàn cảnh từ đi làm đến đi chơi. Áo có form thân và tay hơi ôm vào người nhưng không bó sát cơ thể, mặc vừa vai tôn nên vóc dáng mạnh mẽ. Đặc biệt áo thiết kế phối màu phá cách giữa phần thân và tay áo mạng lại sự trẻ trung, cá tính.</p><p>Áo polo nam là loại trang phục không thể không có trong tủ đồ của các chàng trai. Nó luôn là món trang phục lý tưởng cho những ngày hè oi bức nhưng bên cạnh đó chúng ta cũng có thể sử dụng áo polo linh hoạt vào bất cứ mùa nào trong năm. Chính vì vậy, áo polo nam hội tụ đủ những yếu tố mà các chàng trai cần: lịch sự, chỉn chu và hiện đại.</p>', 0, NULL, N'2aa717ac-f048-4b93-9507-188d0f84138d', N'e4d33c53-b8a3-4f82-9ff3-e611912631fe', N'Ao-Polo-Tay-Bo-Phoi-Mau-Form-Fitted-PS002', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'73913b9e-fa97-46b4-b8dc-2013a97ba592', N'Blazers', NULL, 1, 12, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'9bcb9dca-0595-4bff-b98a-21aed5ed0ff5', N'Outerwear', NULL, 1, 5, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'dd7f55e4-a420-497e-ae56-25ad5330e065', N'Pants', NULL, 1, 11, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'f81c2d8a-8ed2-406f-ac06-3cfcb911682b', N'Tank tops', NULL, 1, 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'668c6874-ab33-4000-9a8f-4d5e00b0642b', N'Shorts', NULL, 1, 10, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'25181eeb-8744-46e9-a970-83d16a4bc28c', N'Hoodies', NULL, 1, 6, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'587b6129-388b-4ed3-93be-92a7f405a466', N'Polo shirts', NULL, 1, 2, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'c7d5c94f-a0f8-4fe3-8fd9-cd01d88cb89d', N'Shirts', NULL, 1, 3, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'e36647ea-db84-448b-9689-d386905b3d67', N'T-Shirts', NULL, 1, 4, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'292a837d-ab92-432d-a0b0-dfd75ae8725c', N'Joggers', NULL, 1, 9, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'e14d5169-a695-4cd0-819e-e98d72885ece', N'Jeans', NULL, 1, 8, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'64af9562-e358-49fc-9fbc-eca3386a4489', N'Accessories & Jewelry', NULL, 1, 14, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'36b9f847-f733-4536-8326-ee74309b5883', N'Sweatshirts', NULL, 1, 7, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[ProductCategory] ([categoryId], [categoryName], [meta], [status], [order], [dateCreate]) VALUES (N'15a5870b-7cd3-4fda-b3e8-f9d1fe8e01b1', N'Shoes & Bags', NULL, 1, 13, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
INSERT [dbo].[Role] ([roleId], [roleName], [meta], [status], [dateCreate]) VALUES (N'54ed1855-5103-4121-811c-3997ce4c2241', N'Customer Member', N'CustomerMember', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Role] ([roleId], [roleName], [meta], [status], [dateCreate]) VALUES (N'b69f924e-629c-4b58-b051-9634f4a12604', N'Employee', N'Employee', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Role] ([roleId], [roleName], [meta], [status], [dateCreate]) VALUES (N'bcfce6d7-bc21-498a-95d4-b138332de823', N'Customer', N'Customer', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
INSERT [dbo].[Role] ([roleId], [roleName], [meta], [status], [dateCreate]) VALUES (N'b14d0769-e485-4455-93bc-c39007910115', N'Manager', N'Manager', 1, CAST(N'2024-02-18T14:04:00' AS SmallDateTime))
GO
ALTER TABLE [dbo].[Article] ADD  DEFAULT (newid()) FOR [articleId]
GO
ALTER TABLE [dbo].[Article] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[Contact] ADD  DEFAULT (newid()) FOR [contactId]
GO
ALTER TABLE [dbo].[Contact] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateContact]
GO
ALTER TABLE [dbo].[Invoince] ADD  DEFAULT (newid()) FOR [invoinceId]
GO
ALTER TABLE [dbo].[Invoince] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[InvoinceDetail] ADD  DEFAULT ((0)) FOR [quanlity]
GO
ALTER TABLE [dbo].[InvoinceDetail] ADD  DEFAULT ((0)) FOR [price]
GO
ALTER TABLE [dbo].[InvoinceDetail] ADD  DEFAULT ((0)) FOR [discount]
GO
ALTER TABLE [dbo].[Member] ADD  DEFAULT (newid()) FOR [memberId]
GO
ALTER TABLE [dbo].[Member] ADD  DEFAULT (N'~/Content/img/avatar.png') FOR [avatar]
GO
ALTER TABLE [dbo].[Member] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[Menu] ADD  DEFAULT (newid()) FOR [menuId]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_status]  DEFAULT ((0)) FOR [status]
GO
ALTER TABLE [dbo].[Menu] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT (newid()) FOR [productId]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__price__3F466844]  DEFAULT ((0)) FOR [price]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__discoun__403A8C7D]  DEFAULT ((0)) FOR [discount]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__quanlit__412EB0B6]  DEFAULT ((0)) FOR [quanlity]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF__Product__brand__4222D4EF]  DEFAULT ('No brand') FOR [brand]
GO
ALTER TABLE [dbo].[Product] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[ProductCategory] ADD  DEFAULT (newid()) FOR [categoryId]
GO
ALTER TABLE [dbo].[ProductCategory] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (newid()) FOR [roleId]
GO
ALTER TABLE [dbo].[Role] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[Slide] ADD  DEFAULT (newid()) FOR [slideId]
GO
ALTER TABLE [dbo].[Slide] ADD  DEFAULT (CONVERT([smalldatetime],getdate())) FOR [dateCreate]
GO
ALTER TABLE [dbo].[InvoinceDetail]  WITH CHECK ADD  CONSTRAINT [FK_INVOINCEDETAIL_INVOINCE] FOREIGN KEY([invoinceId])
REFERENCES [dbo].[Invoince] ([invoinceId])
GO
ALTER TABLE [dbo].[InvoinceDetail] CHECK CONSTRAINT [FK_INVOINCEDETAIL_INVOINCE]
GO
ALTER TABLE [dbo].[InvoinceDetail]  WITH CHECK ADD  CONSTRAINT [FK_INVOINCEDETAIL_PRODUCT] FOREIGN KEY([productId])
REFERENCES [dbo].[Product] ([productId])
GO
ALTER TABLE [dbo].[InvoinceDetail] CHECK CONSTRAINT [FK_INVOINCEDETAIL_PRODUCT]
GO
USE [master]
GO
ALTER DATABASE [menfs] SET  READ_WRITE 
GO
