use Schimb_Valutar
go
insert into Clienti
	values ('c000001', 1234567890123, 'Popescu', 'Ion', 'Str. Florilor nr. 10', 12345678, 'popescu@example.com'),
		   ('c000002', 9876543210987, 'Ionescu', 'Maria', 'Bd. Libertății nr. 25', 98765432, 'maria@example.com'),
		   ('c000003', 1112223334445, 'Constantinescu', 'Ana', 'Splaiul Independenței nr. 5', 11222333, 'ana@example.com'),
		   ('c000004', 5554443332221, 'Dumitrescu', 'Andrei', 'Bd. Decebal nr. 17', 55544333, 'andrei@example.com'),
		   ('c000005', 9998887776660, 'Popa', 'Elena', 'Str. Mihai Viteazu nr. 8', 99988777, 'elena@example.com'),
		   ('c000006', 7778889991112, 'Georgescu', 'Mihai', 'Piața Unirii nr. 3', 77888999, 'mihai@example.com'),
		   ('c000007', 3332221110004, 'Stanciu', 'Ana-Maria', 'Calea Victoriei nr. 12', 33222111, 'anamaria@example.com'),
		   ('c000008', 2223334445556, 'Iftime', 'George', 'Bd. Timișoara nr. 20', 22233444, 'george@example.com'),
		   ('c000009', 6667778889993, 'Gheorghe', 'Andreea', 'Aleea Trandafirilor nr. 6', 66777888, 'andreea@example.com'),
		   ('c000010', 8889990001117, 'Dobre', 'Alexandru', 'Calea Dorobanților nr. 15', 88999000, 'alexandru@example.com');
go
exec insertTranzactii 't000001', 'c000001', 's001', 100