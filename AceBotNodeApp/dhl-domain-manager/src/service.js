const express = require('express');
const bodyParser = require('body-parser');

const RetrieveParcelInfo = require('./retrieve-parcel-info');
const UpdateParcelInfo = require('./update-parcel-info');
const CreateParcel = require('./create-parcel');
const _ = require('lodash');

const app = express();
app.use(bodyParser.json());

app.get('/serviceLocations/:city', (req, res) => {
    RetrieveParcelInfo.fetchServiceLocations(req.params.city)
        .then(serviceLocations => {
            res.send(200, serviceLocations);
        })
        .catch(error => {
            res.send(400, error);
        })
});

app.get('/parcelInfo/:id', (req, res) => {
    const id = `orderid-${req.params.id}`;
    RetrieveParcelInfo.fetchDetails(id)
        .then(parcelInfo => {
            res.send(200, parcelInfo);
        })
        .catch(error => {
            res.send(400, error);
        })
});

app.post('/createParcel', (req, res) => {
    //generate order id
    //console.log("Create req received"+ JSON.stringify(req));
    const id = _.random(1234567890, 9876543210);
    const orderDetails = req.body;
    //update order details with tracking id
    orderDetails.tracking.code = id;
    orderDetails.id = id;
    //Add default status to the new order
    orderDetails.tracking.current_status = "The instruction data for this shipment have been provided by the sender to DHL electronically";
    CreateParcel.generateOrder(`orderid-${id}`, orderDetails)
        .then(orderResponse => {
            res.send(200, {"orderId": id});
        })
        .catch(error => {
            res.send(400, error);
        })
});

app.post('/updateParcelInfo/:id', (req, res) => {
    const id = `orderid-${req.params.id}`;
    const detailsToUpdate = req.body;
    UpdateParcelInfo.updateDetails(id, detailsToUpdate)
        .then(() => {
            res.send(200, {"status": "Details Updated"});
        })
        .catch(error => {
            res.send(400, error);
        })
});

app.listen('1111', () => {
    console.log('Node Test Service Started and listening on port 1111')
});