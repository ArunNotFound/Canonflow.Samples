import React from 'react';
import { useForm } from 'react-hook-form';
import * as Validators from '../validators';

export default function WalletsForm() {
  const { register, handleSubmit, formState: { errors } } = useForm();

  const onSubmit = (data: any) => {
    console.log("Validated Data:", data);
  };

  return (
    <div className="max-w-xl mx-auto mt-10">
      <form onSubmit={handleSubmit(onSubmit)} className="p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl border border-gray-100">
        <div className="mb-8">
          <h2 className="text-3xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-blue-600 to-indigo-600 tracking-tight">WalletsForm</h2>
          <p className="text-gray-500 mt-2 text-sm">Mathematically sound data entry.</p>
        </div>
        
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700">wallet_id</label>
        <input 
          type="text" 
          {...register('wallet_id')} 
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm transition duration-150 ease-in-out hover:border-blue-400"
        />
        {errors.wallet_id && <p className="text-red-500 text-xs mt-1">{(errors.wallet_id as any).message}</p>}
      </div>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700">customer_id</label>
        <input 
          type="text" 
          {...register('customer_id')} 
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm transition duration-150 ease-in-out hover:border-blue-400"
        />
        {errors.customer_id && <p className="text-red-500 text-xs mt-1">{(errors.customer_id as any).message}</p>}
      </div>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700">currency</label>
        <input 
          type="text" 
          {...register('currency')} 
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm transition duration-150 ease-in-out hover:border-blue-400"
        />
        {errors.currency && <p className="text-red-500 text-xs mt-1">{(errors.currency as any).message}</p>}
      </div>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700">status</label>
        <input 
          type="text" 
          {...register('status', { validate: (v, formValues) => Validators.validate_wallets_status(formValues) || 'Invalid value' })} 
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm transition duration-150 ease-in-out hover:border-blue-400"
        />
        {errors.status && <p className="text-red-500 text-xs mt-1">{(errors.status as any).message}</p>}
      </div>
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700">created_at</label>
        <input 
          type="text" 
          {...register('created_at')} 
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm transition duration-150 ease-in-out hover:border-blue-400"
        />
        {errors.created_at && <p className="text-red-500 text-xs mt-1">{(errors.created_at as any).message}</p>}
      </div>

        <button type="submit" className="w-full mt-6 px-4 py-3 font-semibold bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-xl shadow-md hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 transition-all transform hover:scale-[1.02]">
          Submit Securely
        </button>
      </form>
    </div>
  );
}
