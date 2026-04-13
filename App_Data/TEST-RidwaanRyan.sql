SELECT 
  c.CustomerID,
  c.CompanyName,
  c.enabled,
  c.PredictionDisabled,
  c.EmailAddress,
  cu.NextCoffeeBy,
  DateDiff("d", Now(), cu.NextCoffeeBy) AS DaysUntilNextCoffee,
  nr.NextDeliveryDate,
  DateDiff("d", Now(), nr.NextDeliveryDate) AS DaysUntilDelivery,
  '--- FILTER RESULTS ---' AS Separator,
  IIF(cu.NextCoffeeBy >= DateAdd("d", 6, Now()) AND cu.NextCoffeeBy <= DateAdd("d", 10, Now()), 'PASS', 'FAIL - NextCoffeeBy outside 6-10 day window') AS WindowCheck,
  IIF(c.enabled = True, 'PASS', 'FAIL') AS EnabledCheck,
  IIF(c.PredictionDisabled = False, 'PASS', 'FAIL') AS PredictionCheck,
  IIF((c.EmailAddress IS NOT NULL AND c.EmailAddress <> ''), 'PASS', 'FAIL') AS EmailCheck
FROM (CustomersTbl AS c
  INNER JOIN ClientUsageTbl AS cu ON c.CustomerID = cu.CustomerID)
  LEFT JOIN NextRoastDateByCityTbl AS nr ON c.City = nr.CityID
WHERE c.CompanyName LIKE '%Ridwaan%' OR c.CompanyName LIKE '%Akhalwaya%' OR c.CompanyName LIKE '%Ryan%' OR c.CompanyName LIKE '%Vermooten%'
