UPDATE    DynEntityAttribConfig
SET              ActiveVersion = 0
WHERE     (DynEntityAttribConfigUid IN
                          (SELECT     uid
                            FROM          (SELECT     MIN(DynEntityAttribConfigUid) AS uid, DynEntityAttribConfigId
                                                    FROM          DynEntityAttribConfig AS DynEntityAttribConfig_2
                                                    WHERE      (DynEntityAttribConfigId IN
                                                                               (SELECT     DynEntityAttribConfigId
                                                                                 FROM          (SELECT     DynEntityAttribConfigId, COUNT(DynEntityAttribConfigId) AS cnt
                                                                                                         FROM          DynEntityAttribConfig AS DynEntityAttribConfig_1
                                                                                                         WHERE      (ActiveVersion = 1)
                                                                                                         GROUP BY DynEntityAttribConfigId) AS subq
                                                                                 WHERE      (cnt > 1))) AND (ActiveVersion = 1)
                                                    GROUP BY DynEntityAttribConfigId) AS subqtwo))