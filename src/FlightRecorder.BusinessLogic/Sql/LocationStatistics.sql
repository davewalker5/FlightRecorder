SELECT  l.Name,
        COUNT( DISTINCT s.Id ) AS "Sightings",
        COUNT( DISTINCT f.Id ) AS "Flights",
        COUNT( DISTINCT ai.Id ) AS "Aircraft",
        COUNT( DISTINCT m.Id ) AS "Models",
        COUNT( DISTINCT ma.Id ) AS "Manufacturers"
FROM AIRLINE a
INNER JOIN FLIGHT f ON f.Airline_Id = a.Id
INNER JOIN SIGHTING s ON s.Flight_Id = f.Id
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN AIRCRAFT ai ON ai.Id = s.Aircraft_Id
INNER JOIN MODEL m ON m.Id = ai.Model_Id
INNER JOIN MANUFACTURER ma ON ma.Id = m.Manufacturer_Id
WHERE s.Date BETWEEN '$from' AND '$to'
GROUP BY l.Name
ORDER BY l.Name ASC;