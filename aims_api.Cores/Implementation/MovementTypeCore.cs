using aims_api.Cores.Interface;
using aims_api.Enums;
using aims_api.Models;
using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Cores.Implementation
{
    public class MovementTypeCore : IMovementTypeCore
    {
        private IMovementTypeRepository MovementTypeRepo { get; set; }
        public MovementTypeCore(IMovementTypeRepository movementTypeRepo)
        {
            MovementTypeRepo = movementTypeRepo;
        }

        public async Task<RequestResponse> GetMovementTypePg(int pageNum, int pageItem)
        {   
            var data = await MovementTypeRepo.GetMovementTypePg(pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetMovementTypePgSrch(string searchKey, int pageNum, int pageItem)
        {
            var data = await MovementTypeRepo.GetMovementTypePgSrch(searchKey, pageNum, pageItem);

            if (data != null && data.Any())
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> GetMovementTypeById(string movementTypeId)
        {
            var data = await MovementTypeRepo.GetMovementTypeById(movementTypeId);

            if (data != null)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record found.", data);
            }

            return new RequestResponse(ResponseCode.FAILED, "No record found.");
        }

        public async Task<RequestResponse> CreateMovementType(MovementTypeModel movementType)
        {
            bool movementTypeExists = await MovementTypeRepo.MovementTypeExists(movementType.MovementTypeId);
            if (movementTypeExists)
            {
                return new RequestResponse(ResponseCode.FAILED, "Similar MovementTypeId exists.");
            }

            bool res = await MovementTypeRepo.CreateMovementType(movementType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record created successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to create record.");
        }

        public async Task<RequestResponse> UpdateMovementType(MovementTypeModel movementType)
        {
            bool res = await MovementTypeRepo.UpdateMovementType(movementType);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record updated successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to update record.");
        }

        public async Task<RequestResponse> DeleteMovementType(string movementTypeId)
        {
			// place item in use validator here

            bool res = await MovementTypeRepo.DeleteMovementType(movementTypeId);
            if (res)
            {
                return new RequestResponse(ResponseCode.SUCCESS, "Record deleted successfully.");
            }

            return new RequestResponse(ResponseCode.FAILED, "Failed to delete record.");
        }
    }
}
