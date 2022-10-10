USE [Posterr]
GO

/****** Object:  Table [dbo].[Posts]    Script Date: 10/05/2022 17:05:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Posts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [varchar](777) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[Type] [varchar](20) NOT NULL,
	[PostOriginId] [int] NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Posts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK_Post_Post] FOREIGN KEY([PostOriginId])
REFERENCES [dbo].[Posts] ([Id])
GO

ALTER TABLE [dbo].[Posts] CHECK CONSTRAINT [FK_Post_Post]
GO

ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK_Post_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[Posts] CHECK CONSTRAINT [FK_Post_User]
GO


