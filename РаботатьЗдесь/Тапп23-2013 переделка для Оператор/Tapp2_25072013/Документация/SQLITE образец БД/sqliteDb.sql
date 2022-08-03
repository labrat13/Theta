DROP TABLE IF EXISTS `CellTable`;
CREATE TABLE "CellTable"(     "id" Integer Primary Key Autoincrement  NOT NULL  ,      "name" Text  NOT NULL  ,      "descr" Text  NOT NULL  ,      "active" Integer  NOT NULL   DEFAULT (0),      "type" Integer  NOT NULL   DEFAULT (0),      "creatime" Integer  NOT NULL   DEFAULT (0),      "moditime" Integer  NOT NULL   DEFAULT (0),      "ronly" Integer  NOT NULL   DEFAULT (0),      "state" Integer   DEFAULT (0),      "sflag" Integer  NOT NULL   DEFAULT (0),      "val" Blob   DEFAULT ('NULL'),      "valtype" Integer  NOT NULL   DEFAULT (0),      "cellid" Integer  NOT NULL   DEFAULT (0));
DROP TABLE IF EXISTS `sqlite_sequence`;
CREATE TABLE sqlite_sequence(name,seq);
DROP TABLE IF EXISTS `LinkTable`;
CREATE TABLE "LinkTable"(     "id" Integer Primary Key Autoincrement  ,      "downID" Integer  NOT NULL   DEFAULT (0),      "upID" Integer  NOT NULL   DEFAULT (0),      "axis" Integer  NOT NULL   DEFAULT (0),      "state" Integer  NOT NULL   DEFAULT (0),      "active" Integer  NOT NULL   DEFAULT (0),      "sflag" Integer  NOT NULL   DEFAULT (0),      "descr" Text  ,      "moditime" Integer  NOT NULL   DEFAULT (0));
DROP TABLE IF EXISTS `EngineTable`;
CREATE TABLE "EngineTable"(     "id" Integer Primary Key Autoincrement  ,      "version" Text  NOT NULL  ,      "step" Text  NOT NULL  ,      "lognum" Integer  NOT NULL   DEFAULT (0),      "loglevel" Integer  NOT NULL   DEFAULT (0),      "descr" Text  ,      "name" Text  NOT NULL  ,      "sflag" Integer  NOT NULL   DEFAULT (0),      "state" Integer  NOT NULL   DEFAULT (0),      "cellmode" Integer  NOT NULL   DEFAULT (0),      "idcon" Integer  NOT NULL   DEFAULT (0));

CREATE INDEX index_CellTable_Name ON CellTable(name ASC)
CREATE UNIQUE INDEX index_CellTable_CellId ON CellTable(cellid ASC)
CREATE INDEX index_LinkTable_upId ON LinkTable(upID ASC)
CREATE INDEX index_LinkTable_downId ON LinkTable(downID ASC)


