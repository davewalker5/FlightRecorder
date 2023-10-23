SELECT  STRFTIME('%Y', s.Date ) AS "Year",
        STRFTIME('%m', s.Date ) AS "Month",
        COUNT( DISTINCT s.Id ) AS "Sightings",
        COUNT( DISTINCT f.Id ) AS "Flights"
FROM AIRLINE a
INNER JOIN FLIGHT f ON f.Airline_Id = a.Id
INNER JOIN SIGHTING s ON s.Flight_Id = f.Id
WHERE s.Date BETWEEN '$from' AND '$to'
GROUP BY STRFTIME('%Y', s.Date ), STRFTIME('%m', s.Date )
ORDER BY s.Date ASC;
