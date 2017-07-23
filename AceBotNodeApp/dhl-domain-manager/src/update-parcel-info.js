const couchbaseUtils = require('./couchbase-utils');
const _ = require('lodash');
const patch = require('jsonpatch');
const flatten = require('flat');

const _createPatchElement = (key, value) => {
  key = _.replace(key, /\./g, '/');
  return {
    op: "replace",
    path: `/${key}`,
    value: value
  }
};

const updateDetails = (parcelId,detailsToUpdate) => {
  return couchbaseUtils.getRecord(parcelId)
    .then(rawParcelDetails => {
      const patchElements = [];
      const flattenedDetailsToUpdate = flatten(detailsToUpdate);

      _.forEach(Object.keys(flattenedDetailsToUpdate), key => {
        patchElements.push(_createPatchElement(key, flattenedDetailsToUpdate[key]));
      });

      const updatedParcelDetails = patch.apply_patch(rawParcelDetails, patchElements);
      couchbaseUtils.postRecord(parcelId, updatedParcelDetails)
        .then(parcelInfo => {
          console.log(`Parcel Info for Id ${parcelId} is ${JSON.stringify(parcelInfo)}`);
          return Promise.resolve(parcelInfo);
        })
        .catch(error => {
          console.log(`Error ${error} while fetching Parcel Info for Id ${parcelId}`);
          return Promise.reject(error);
        })
    });
};

module.exports = {updateDetails};