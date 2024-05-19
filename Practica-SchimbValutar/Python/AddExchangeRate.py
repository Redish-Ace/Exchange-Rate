import requests # type: ignore
import pyodbc # type: ignore
import sys

conn_str = (
    r'DRIVER={SQL Server};'
    r'Server=DESKTOP-N5UB5V3\SQLEXPRESS;'
    r'DATABASE=Schimb_Valutar;'
    r'Trusted_Connection=yes;'
)
cnxn = pyodbc.connect(conn_str)

cursor = cnxn.cursor()

insert_converter = "INSERT INTO Schimb (ID, ID_Valuta_Convertita, ID_Valuta, Schimb) VALUES (?, ?, ?, ?)"
update_converter = "UPDATE Schimb SET Schimb = (?) WHERE ID_Valuta_Convertita = (?) AND ID_Valuta = (?)"
select = "SELECT COUNT(*) FROM Schimb"
selectID = "SELECT ID FROM Valuta WHERE Cod = (?)"
delete = "DELETE FROM Schimb"

cursor.execute(delete)

cursor.execute(select)

result = cursor.fetchone()[0]

id_curr = []
id_conv = []
exch_rate = []

currency_by_iso = [["CHF", "Swiss Franc"], ["EUR", "Euro"], ["GBP", "Pound Sterling"], ["MDL", "Moldovan Leu"], ["RON", "Romanian Leu"],
                   ["RUB", "Russian Ruble"],["UAH", "Ukrainian Hryvnia"], ["USD", "United States Dollar"]];

#api_key = "67c01057be2394edea8a4b31"
#url = f"https://v6.exchangerate-api.com/v6/{api_key}/latest/"

api_key = "986bed0817b54d7baf9e02d423ad82e5"
url = f"https://api.currencyfreaks.com/v2.0/rates/latest?apikey={api_key}"

exchange_rates = {}
available_currencies = []
currency_list = []
rate=[]

try:
    #for currency in currency_by_iso:
    #    url += currency[0]
    #    response = requests.get(url)
    #    data = response.json()
    #    exchange_rates.update(data['conversion_rates'])
    #    available_currencies += list(exchange_rates.keys())
    #    currency_list += []
    #    url=f"https://v6.exchangerate-api.com/v6/{api_key}/latest/"

    response = requests.get(url)
    data = response.json()
    exchange_rates.update(data["rates"])
    for rate in data['rates']:
        for currency in currency_by_iso:
            if rate == currency[0]:
                available_currencies.append(rate)
                currency_list = []

    values = []

    def create_id(index):
        try:
            if(index>=1 and index<=9):
                id_ = "s00" + str(index);
            elif(index>=10 and index<=99):
                id_ = "s0" + str(index);
            else:
                id_ = "s" + str(index);
            return id_
        except Exception as e:
            print(e)

    for currency in available_currencies:
        original_amount = 1 / float(exchange_rates[currency])
        for convert in available_currencies:
            converted_amount = original_amount * float(exchange_rates[convert])
            if currency not in id_curr: id_curr.append(currency)
            if convert not in id_conv: id_conv.append(convert)
            exch_rate.append(converted_amount)

    id_curr1 = ""
    id_conv1 = ""

    k=0;

    if result == 0:
        for ids1 in id_curr:
            cursor.execute(selectID, ids1)
            id_curr1 = cursor.fetchone()[0]

            for ids2 in id_conv:
                cursor.execute(selectID, ids2)
                id_conv1 = cursor.fetchone()[0]
                #print(id_curr1 + ' ' + ids1 + ' ' + str(exch_rate[k]) + ' ' + id_conv1 + ' ' + ids2)

                values.append((create_id(k+1), id_curr1, id_conv1, exch_rate[k]))
                k+=1;
        cursor.executemany(insert_converter, values)
        #print("Insert Succesful")
except Exception as e:
    exc_type, exc_obj, exc_tb = sys.exc_info()
    print(exc_type, exc_tb.tb_lineno)
    print(e)
finally:
    cnxn.commit()
    cursor.close()
    cnxn.close()
