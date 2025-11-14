USE Prog_A25_Bd_Projet_Prog;
GO
CREATE TRIGGER trg_VerifierPlaceStationnement 
ON stationnementEntreeSortie 
AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @nbVoituresActuelles INT;
	DECLARE @capaciteMax INT;
	DECLARE @reponse NVARCHAR(100);
	DECLARE @IdStationnement INT;
	
	SELECT TOP 1 @capaciteMax = nombrePlaceMax FROM stationnement WHERE numStationnement = @IdStationnement;

	SELECT @nbVoituresActuelles = COUNT(*)
    FROM stationnementEntreeSortie
    WHERE dateSortie IS NULL OR dateSortie > dateEntree;

	IF @nbVoituresActuelles >= @capaciteMax
	BEGIN
		ROLLBACK TRANSACTION;
	END
END;
GO

