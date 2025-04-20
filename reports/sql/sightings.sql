SELECT DATE( s.Date ) AS 'Date', l.Name AS 'Location', a.Name AS 'Airline', f.Number, f.Embarkation, f.Destination,
    ac.Registration, ac.Serial_Number, mo.Name AS 'Model', m.Name AS 'Manufacturer' 
FROM SIGHTING s
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN FLIGHT f ON f.Id = s.Flight_Id
INNER JOIN AIRLINE a ON a.Id = f.Airline_Id
INNER JOIN AIRCRAFT ac ON ac.Id = s.Aircraft_Id
INNER JOIN MODEL mo ON mo.Id = ac.Model_Id
INNER JOIN MANUFACTURER m ON m.Id = mo.Manufacturer_Id;
