USE [Posterr]
GO

/****** Object:  Table [dbo].[Followers]    Script Date: 10/05/2022 17:04:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Followers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[FollowingUserId] [int] NULL,
 CONSTRAINT [PK_Follower] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Followers]  WITH CHECK ADD  CONSTRAINT [FK_Follower_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Followers] CHECK CONSTRAINT [FK_Follower_User]
GO

ALTER TABLE [dbo].[Followers]  WITH CHECK ADD  CONSTRAINT [FK_Follower_User1] FOREIGN KEY([FollowingUserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Followers] CHECK CONSTRAINT [FK_Follower_User1]
GO


