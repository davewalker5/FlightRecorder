SELECT      s.Date,
            a.Name AS "Airline",
            f.Number,
            f.Embarkation,
            f.Destination,
            ac.Registration,
            m.Name AS "Model",
            ma.Name AS "Manufacturer"
FROM        SIGHTING s
INNER JOIN  FLIGHT f on f.Id = s.Flight_Id
INNER JOIN  AIRLINE a on a.Id = f.Airline_Id
INNER JOIN  AIRCRAFT ac ON ac.Id = s.Aircraft_Id
INNER JOIN  MODEL m ON m.Id = ac.Model_Id
INNER JOIN  MANUFACTURER ma ON ma.Id = m.Manufacturer_Id
WHERE       s.Is_My_Flight = 1
AND         s.Date BETWEEN '$from' AND '$to'
ORDER BY    s.Date;
