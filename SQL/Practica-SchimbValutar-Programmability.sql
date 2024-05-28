use Schimb_Valutar
go
--Vedere care afiseaza codul si denumirea valutei convertire, suma schimbului si codul si denumirea valutei in care se converteste
drop view if exists getRate
go
create view getRate as
	select s.ID, vc.Cod + ': ' + vc.Denumire as Valuta_Convertita, s.Schimb, v.Cod + ': ' + v.Denumire as Valuta
		from Schimb s
		join Valuta vc on vc.ID = s.ID_Valuta_Convertita
		join Valuta v on v.ID = s.ID_Valuta
go
select * from getRate
select * from Valuta
select * from Schimb

--Vedere care afiseaza tranzactii
drop view if exists tranzactii
go
create view tranzactii as
	select t.ID, c.Nume + ' ' + c.Prenume as Client, s.Valuta_Convertita, s.Schimb, s.Valuta, t.Suma, (t.Suma * s.Schimb - t.Suma * s.Schimb * 0.02) as Suma_Finala, (t.Suma * s.Schimb * 0.02) as Comision, Data_tranz
		from Tranzactie t
		join Clienti c on c.ID = t.ID_Client
		join SchimbVechi s on s.ID = t.ID_Schimb
go
select * from tranzactii
--Vedere care afiseaza tranzactii care au fost facute azi
drop view if exists tranzactiiAzi
go
create view tranzactiiAzi as
	select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_Finala * r.Schimb as Suma_finala_lei, Comision * r.Schimb as Comision_lei, Data_tranz
		from tranzactii t
		join getRate r on t.Valuta=r.Valuta_Convertita
			where r.Valuta like 'MDL%' and Data_tranz = convert(varchar, GETDATE(), 23)
go
select * from tranzactiiAzi
--Vederea care afiseaza valuta cea mai solicitata

drop view if exists valutaSolicitata
go
create view valutaSolicitata as
	select ID, Cod, Denumire
		from Valuta v
			where exists ( select ID_Valuta from Schimb s
							where s.ID_Valuta = v.ID and s.ID in ( select top 1 ID_Schimb from Tranzactie
								group by ID_Schimb
									order by count(*) desc))
go
select * from valutaSolicitata

--Vedere care afiseaza cea mai avantajoasa tranzactie in lei

drop view if exists tranzactieAvantaj
go
create view tranzactieAvantaj as
    select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_Finala * r.Schimb as Suma_finala_lei, Comision * r.Schimb as Comision_lei, Data_tranz
		from tranzactii t
		join getRate r on t.Valuta=r.Valuta_Convertita
			where r.Valuta like 'MDL%'
go
select top 1 * from tranzactieAvantaj
	order by Suma_finala_lei desc

--Vedere care afiseaza comisionul total
drop view if exists comisionTotal
go
create view comisionTotal as
    select sum(Comision_lei) as Comision_Total
		from tranzactieAvantaj
go
select * from comisionTotal
--Crearea unei functii care afiseaza tranzactiile din data introdusa in ordine descrescatoare sumei
go
drop function if exists tranzactiiData
go
create function tranzactiiData(@date date)
returns table
as
	return (select t.ID, Client, t.Valuta_Convertita, t.Schimb, t.Valuta, Suma, Suma_Finala, Comision, Suma_finala_lei, Comision_lei, Data_tranz
		from tranzactieAvantaj t
			where Data_tranz = @date)
go
select * from tranzactiiData('2024-05-14')
	order by Suma_finala_lei desc
--Crearea unei proceduri ce insereaza in tabelul Tranzactie datele introduse la executarea procedurii si introducerea datii tranzactiei prin functia getdate() prin update
go
drop procedure if exists insertTranzactii
go
create procedure insertTranzactii
@id nvarchar(7),
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@id_schimb_vechi nvarchar(7),
@suma money,
@date date
as
begin
	if (@id_schimb is not null)
	begin
		if not exists (select 'false' from SchimbVechi where Valuta_Convertita = (select r.Valuta_Convertita from getRate r where r.ID = @id_schimb) 
		and Valuta = (select r.Valuta from getRate r where r.ID = @id_schimb) 
		and Schimb = (select r.Schimb from getRate r where r.ID = @id_schimb))
		begin
			insert into SchimbVechi (ID, Valuta_Convertita, Schimb, Valuta)
				select @id_schimb_vechi, r.Valuta_Convertita, r.Schimb, r.Valuta from getRate r
					where r.ID = @id_schimb
		end;
	end
	
	declare @id_sv nvarchar(7)
	select @id_sv = ID from SchimbVechi where Valuta_Convertita = (select r.Valuta_Convertita from getRate r where r.ID = @id_schimb) 
		and Valuta = (select r.Valuta from getRate r where r.ID = @id_schimb) 
		and Schimb = (select r.Schimb from getRate r where r.ID = @id_schimb)

	if(@id_sv is null)
	begin
		set @id_sv = @id_schimb_vechi
	end;

	if (@id is not null) and (@id_client is not null) and (@suma != 0)
	begin
		insert into Tranzactie (ID, ID_Client, ID_Schimb, Suma) 
			values(@id, @id_client, @id_sv, @suma)
			
		if(@date is null)
		begin
			update Tranzactie
				set Data_tranz = getdate()
					where ID = @id
		end;
		else
		begin
			update Tranzactie
					set Data_tranz = @date
						where ID = @id
		end;
	end;
end
select * from Tranzactie
exec insertTranzactii 't000011', 'c000004', 's045', 's000101', 200, null

exec insertTranzactii 't000012', 'c000005', 's046', 's000102', 200, null
--Crearea unei proceduri ce modifica datele din tabelul Tranzactie
go
drop procedure if exists updateTranzactii
go
create procedure updateTranzactii
@id nvarchar(7),
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@id_schimb_vechi nvarchar(7),
@suma money
as
begin
	if (@id is not null) 
	begin
		if (@id_client != '') 
		begin
			update Tranzactie
				set ID_Client = @id_client
					where ID = @id
		end;
		if (@id_schimb != '') 
		begin
			if not exists (select 'false' from SchimbVechi where Valuta_Convertita = (select Valuta_Convertita from getRate where ID = @id_schimb) and Valuta = (select Valuta from getRate where ID = @id_schimb))
			begin
				insert into SchimbVechi (ID, Valuta_Convertita, Schimb, Valuta)
					select @id_schimb_vechi, r.Valuta_Convertita, r.Schimb, r.Valuta from getRate r
						where r.ID = @id_schimb
			end;
			update Tranzactie
				set ID_Schimb = (select ID from SchimbVechi where Valuta_Convertita = (select Valuta_Convertita from getRate where ID = @id_schimb) and Valuta = (select Valuta from getRate where ID = @id_schimb))
					where ID = @id
		end;
		if (@suma != 0) 
		begin
			update Tranzactie
				set Suma = @suma
					where ID = @id
		end;
	end;
end
go
exec updateTranzactii 't000001', null, 's016', 's000002', 0
go
select * from tranzactii
go
select * from SchimbVechi
--Crearea unei fucntii care afiseaza id-ul tranzactiei dupa client, schimb, suma si data
go
drop function if exists getTranzactii
go
create function getTranzactii(
@id_client nvarchar(7),
@id_schimb nvarchar(7),
@suma money)
returns @id table (ID nvarchar(7))
as
begin
	declare @id_schimb_vechi nvarchar(7)
	select @id_schimb_vechi = ID from SchimbVechi where Valuta_Convertita = (select Valuta_Convertita from getRate where ID = @id_schimb) and Valuta = (select Valuta from getRate where ID = @id_schimb)

	insert into @id
		select ID from Tranzactie where (ID_Client = @id_client) or (ID_Schimb = @id_schimb_vechi) or (Suma = @suma)
	return
end
go
--
drop procedure deleteSchimbV
go
create procedure deleteSchimbV
@id_trans nvarchar(7)
as
begin
	if(1 >= (select count(ID_Schimb) from Tranzactie where ID_Schimb = (select ID_Schimb from Tranzactie where ID = @id_trans) group by ID_Schimb))
	delete from SchimbVechi
		where Valuta_Convertita = (select t.Valuta_Convertita from tranzactii t where t.ID = @id_trans) 
		and Valuta = (select t.Valuta from tranzactii t where t.ID = @id_trans) 
		and Schimb = (select t.Schimb from tranzactii t where t.ID = @id_trans)
end
go
exec deleteSchimbV 't000011'
--Crearea unei proceduri care insereaza date despre clienti
go
drop procedure if exists insertClienti
go
create procedure insertClienti
@id nvarchar(7),
@idnp bigint,
@nume nvarchar(25),
@prenume nvarchar(25),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50)
as
begin
	if (@id is not null) and (@idnp != 0) and (@nume is not null) and (@prenume is not null) and (@adresa is not null) and (@telefon != 0) and (@email is not null)
	begin
		insert into Clienti 
			values(@id, @idnp, @nume, @prenume, @adresa, @telefon, @email)
	end;
end

select * from Clienti
--Crearea unei proceduri ce modifica datele din tabelul Tranzactie
go
drop procedure if exists updateClienti
go
create procedure updateClienti
@id nvarchar(7),
@nume nvarchar(50),
@prenume nvarchar(50),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50)
as
begin
	if (@id is not null) 
	begin
		if (@nume != '') 
		begin
			update Clienti
				set Nume = @nume
					where ID = @id
		end;
		if (@prenume != '') 
		begin
			update Clienti
				set Prenume = @prenume
					where ID = @id
		end;
		if (@adresa != '') 
		begin
			update Clienti
				set Adresa = @adresa
					where ID = @id
		end;
		if (@telefon != 0) 
		begin
			update Clienti
				set Telefon = @telefon
					where ID = @id
		end;
		if (@email != '') 
		begin
			update Clienti
				set Email = @email
					where ID = @id
		end;
	end;
end
--Crearea unei proceduri ce sterge datele din tabelul Clienti
go
drop function if exists getClient
go
create function getClient(
@idnp bigint,
@nume nvarchar(50),
@prenume nvarchar(50),
@adresa nvarchar(50),
@telefon bigint,
@email nvarchar(50))
returns @id table (ID nvarchar(7))
as
begin
	insert into @id
		select ID from Clienti where (IDNP = @idnp) or (Nume = @nume) or (Prenume = @prenume) or (Adresa = @adresa) or (Telefon = @telefon) or (Email = @email)
	return
end
go
select * from getClient(0, 'Ginga', 'Lilian', '', 0, '')