import os
import re
from sentinelhub import SentinelHubRequest, DataCollection, MimeType, CRS, BBox, SHConfig, Geometry, bbox_to_dimensions
import psycopg2

conn = psycopg2.connect(dbname='GazonNDVI', user='postgres', 
                        password='Rbhf080166', host='localhost', port=5432)
cursor = conn.cursor()

id=3

startdate = '2022-04-20'
enddate = '2022-05-20'
'''
startdate = datetime.strptime(startdate, "%Y-%m-%d")
startdate = datetime.now().date()
enddate = datetime.strptime(enddate, "%Y-%m-%d")
enddate = datetime.now().date()
'''

cursor.execute('SELECT coordinates FROM field WHERE id =%s',(id,)) 
for row in cursor:
    print(row)
 

str='' 
for i in row: 
    str=str+i 

reg = re.compile('[":a-zA-Z\s|\[|\]]') 
str = reg.sub('', str) #удаление букв и "" :

coord = re.findall(r'\d+\.\d+', str) #поиск float и запись в массив с подстроками

for i in range(len(coord)):
    coord[i]=float(coord[i])

arr = [[coord[i:i + 2] for i in range(0, len(coord), 2)]]

# Credentials
config = SHConfig()
config.sh_client_id = '9b3cd1f8-ce9a-4b5b-8fdd-bacd3cefaefd'
config.sh_client_secret = 'V~6E-fqbsh5Rs_zOY;lO>AvRbwCF,911t?a684Q,'

evalscript = """
//VERSION=3

function setup() {
  return {
    input: ["B02", "B03","B04", "B08"],
    output: { bands: 3 }
  };
}

function evaluatePixel(sample) {
   // return [2.5 * sample.B04, 2.5 * sample.B03, 2.5 * sample.B02];
  let ndvi = (sample.B08 - sample.B04) / (sample.B08 + sample.B04)
    
    /*if (ndvi<-0.2) return [0,0,0]; //ндви с нелепыми цветами
else if (ndvi<-0.1) return [1,0,0];
else if (ndvi<0) return [0.5,0.6,0,0];
else if (ndvi<0.1) return [0.4,0,0];
else if (ndvi<0.2) return [1,1,0.2];
else if (ndvi<0.3) return [0.8,0.8,0.2];
else if (ndvi<0.4) return [0.4,0.4,0];
else if (ndvi<0.5) return [0.2,1,1];
else if (ndvi<0.6) return [0.2,0.8,0.8];
else if (ndvi<0.7) return [0,0.4,0.4];
else if (ndvi<0.8) return [0.2,1,0.2];
else if (ndvi<0.9) return [0.2,0.8,0.2];
else return [0,0.4,0];*/
return colorBlend(ndvi, [0,0.5,1], [[1,1,1],[0,0.5,0],[0,0,0]]) 

}
"""

leng = len(arr[0])
xmin=10000000.0
xmax=0.0
ymin=10000000.0
ymax=0.0

for i in range(leng):
  if(arr[0][i][0] > xmax):
      xmax = arr[0][i][0]
  if(arr[0][i][0]< xmin):
      xmin = arr[0][i][0]
  if(arr[0][i][1] > ymax):
      ymax = arr[0][i][1]
  if(arr[0][i][1]< ymin):
      ymin = arr[0][i][1]
  

bbox = BBox(bbox= [xmin,ymin,xmax,ymax], crs=CRS.WGS84)
geometry = Geometry(geometry={"type":"Polygon","coordinates":arr}, crs=CRS.WGS84)


request = SentinelHubRequest(
    data_folder="satellitePhoto",
    evalscript=evalscript,
    input_data=[
        SentinelHubRequest.input_data(
            data_collection=DataCollection.SENTINEL2_L2A,          
            time_interval=(startdate, enddate),
            other_args={"dataFilter": {"maxCloudCoverage": 17}}
                   
        ),
    ],
    responses=[
        SentinelHubRequest.output_response('default', MimeType.PNG),
    ],
    
    bbox=bbox,
    geometry=geometry,
    size = bbox_to_dimensions(bbox, resolution=1),
    config=config 
)

#response = plot_image(request.get_data(save_data = True, decode_data  = True)[0], factor=3.5 / 255, clip_range=(0, 1))
response = request.get_data(save_data = True, decode_data  = True)

path = request.data_folder
file_ext = r".png"

dir = os.listdir(path)
dir = [os.path.join(path, dirr) for dirr in dir]
dir=(max(dir, key=os.path.getctime))
arrPath = [d for d in os.listdir(dir) if d.endswith(file_ext)]
p=arrPath[0]
path=os.path.join(dir,p)

path=os.path.abspath(p)


cursor.execute('update ndvi set ndvimap= %s where id_field = %s AND startdate = %s AND enddate = %s ',(path,id,startdate, enddate)) 
conn.commit()

conn.close() 