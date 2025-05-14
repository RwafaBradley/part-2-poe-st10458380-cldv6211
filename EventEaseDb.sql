-- DATABASE CREATION

USE master

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'EventEase')
DROP DATABASE EventEase
CREATE DATABASE EventEase

USE EventEase


CREATE TABLE Venue (
    VenueId INT IDENTITY(1,1) PRIMARY KEY,
    VenueName NVARCHAR(255) NOT NULL,
    Locations NVARCHAR(255) NOT NULL,
    Capacity INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    
);

CREATE TABLE Event (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    EventName NVARCHAR(255) NOT NULL,
    EventDate DATE NOT NULL,
    Descriptions NVARCHAR(500),
    VenueId INT NULL, -- Null allows my evetns to be created before assigning a venue
    CONSTRAINT FK_Event_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE SET NULL
);

CREATE TABLE Booking (
    BookingId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL,
    VenueId INT NOT NULL,
    BookingDate DATE NOT NULL,
    CONSTRAINT FK_Booking_Event FOREIGN KEY (EventId) REFERENCES Event(EventId) ON DELETE CASCADE,
    CONSTRAINT FK_Booking_Venue FOREIGN KEY (VenueId) REFERENCES Venue(VenueId) ON DELETE CASCADE,
    CONSTRAINT UQ_Event_Venue UNIQUE (EventId, VenueId)
	-- The last line is for preventiun of duplicate bookings
);



--creating events before venue avilability
INSERT INTO Venue (VenueName, Locations, Capacity, ImageUrl)
VALUES ('Ballortor', 'Las Vegas', 300, 'www.ballortor.com');

INSERT INTO Event (EventName, EventDate, Descriptions, VenueId)
VALUES ('Dance Fest', '2025-09-11', 'A fun dance fest for all to see', 1);

--updating event prior


INSERT INTO Booking (EventId, VenueId,BookingDate)
VALUES (1, 1,GETDATE());


--TABLE MANIPULATION
SELECT *
FROM Venue;

SELECT *
FROM Event;

SELECT *
FROM Booking;


--check why bookingid appeared as 2 when inserting values into venueid